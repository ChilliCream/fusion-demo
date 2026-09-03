export const meta = {
  name: 'nitro-backlog-wave',
  description: 'Drive nitro agent tasks tickets through implement, review, verify, fix cycles; one wave per area, tickets sequential inside a wave',
  whenToUse: 'The orchestrator runs this per wave. args: { actor, repo, branch, maxCycles?, rules?, tooling?, waves: [{ area, worktree, stopOnFailure?, tickets: [{ id, mode?: "implement"|"review", commits?, seedReview?, priorCycles?, worktree?, context? }] }] }. The orchestrator claims tickets before and closes them after; agents never do.',
  phases: [
    { title: 'Implement', detail: 'one implementer per ticket, self-reviews before reporting' },
    { title: 'Review', detail: 'independent reviewer reads the actual diff' },
    { title: 'Verify', detail: 'verifier confirms or dismisses each failed-review finding' },
    { title: 'Fix', detail: 'fixer applies the verified correction plan exactly' },
  ],
}

// Result contracts; mirror ../change-result.schema.json, review-result.schema.json, verification-result.schema.json.
const STR_ARR = { type: 'array', items: { type: 'string' } }
const CHANGE = { type: 'object', additionalProperties: false,
  required: ['status', 'commits', 'verified', 'notVerified', 'escalation'],
  properties: { status: { type: 'string', enum: ['completed', 'blocked', 'failed'] }, commits: STR_ARR, verified: STR_ARR, notVerified: STR_ARR, escalation: { type: ['string', 'null'] } } }
const REVIEW = { type: 'object', additionalProperties: false,
  required: ['status', 'verdict', 'findings', 'verified', 'notVerified', 'escalation'],
  properties: { status: { type: 'string', enum: ['completed', 'blocked', 'failed'] }, verdict: { type: 'string', enum: ['pass', 'fail', 'unknown'] },
    findings: { type: 'array', items: { type: 'object', additionalProperties: false, required: ['id', 'severity', 'summary', 'evidence', 'remediation'],
      properties: { id: { type: 'string' }, severity: { type: 'string', enum: ['blocker', 'major', 'minor'] }, summary: { type: 'string' }, evidence: { type: 'string' }, remediation: { type: 'string' } } } },
    verified: STR_ARR, notVerified: STR_ARR, escalation: { type: ['string', 'null'] } } }
const VERIFY = { type: 'object', additionalProperties: false,
  required: ['status', 'findings', 'correctionPlan', 'verified', 'notVerified', 'escalation'],
  properties: { status: { type: 'string', enum: ['completed', 'blocked', 'failed'] },
    findings: { type: 'array', items: { type: 'object', additionalProperties: false, required: ['id', 'disposition', 'evidence', 'correction'],
      properties: { id: { type: 'string' }, disposition: { type: 'string', enum: ['confirmed', 'dismissed', 'modified'] }, evidence: { type: 'string' }, correction: { type: ['string', 'null'] } } } },
    correctionPlan: STR_ARR, verified: STR_ARR, notVerified: STR_ARR, escalation: { type: ['string', 'null'] } } }

// Role tiers (see references/claude-code.md). Re-derive when lineups change.
const TIER = {
  implement: { model: 'sonnet', effort: 'high' },
  review: { model: 'opus', effort: 'medium' },
  verify: { model: 'fable', effort: 'medium' },
  fix: { model: 'sonnet', effort: 'high' },
}

const A = args || {}
const ACTOR = A.actor
const REPO = A.repo
const BRANCH = A.branch
const MAX_CYCLES = A.maxCycles || 3
const WAVES = A.waves || []
if (!ACTOR || !REPO || !BRANCH || WAVES.length === 0) throw new Error('args.actor, args.repo, args.branch and args.waves are required')

const RULES = `
STANDING RULES (binding):
1. Atomic commits with pathspec: git commit -m "..." -- <files>. Never git add then bare git commit.
2. Formatter on every touched file before committing (write, then check).
3. Never run shared build or test targets concurrently with other agents; restore known side-effect files to HEAD before committing.
4. On a git index.lock failure, wait and retry; never force, never delete the lock.
5. Verify state claims yourself; a report without a commit hash means uncommitted work.
6. Never close tasks, never push, never switch branches, never rebase or merge, never touch files outside your worktree or the user's uncommitted files.
7. Touch only this wave's area; other waves are active in other worktrees.
8. Task comments are binding: read them with nitro agent tasks show <id> --output json before acting. Pass --actor ${ACTOR} on every comment you add.
9. When blocked and no ruling exists, take the most conservative reasonable interpretation, record NEEDS-PLANNER: <question> | chose: <what you did> as a task comment, and continue. Hard-block only when no reasonable interpretation exists.
10. notVerified is mandatory honesty: list every claim you did not exercise.${A.rules ? `
PROJECT RULES:
${A.rules}` : ''}`

const header = (wave, t) => `Repository: ${REPO}, branch ${BRANCH}. Work only inside worktree ${wave.worktree} (a worktree of the same repository; nitro resolves the same workspace from it).
Ticket ${t.id}, area ${wave.area}. Read it first, comments included: nitro agent tasks show ${t.id} --output json (run nitro commands from ${REPO}).${t.context ? `
ORCHESTRATOR CONTEXT (binding): ${t.context}` : ''}${A.tooling ? `
Tooling: ${A.tooling}` : ''}`

const implementPrompt = (wave, t) => `ROLE: Implementer. You implement exactly one ticket.
${header(wave, t)}
Implement exactly the ticket's scope (problem, file scope, fix direction, verification, non-goals). Root-cause fixes only. Anything beyond the ticket is scope creep and fails review. Write or update the focused tests the ticket's verification section demands.
Before reporting, review your own diff the way the external reviewer will: correctness (root cause, not suppression), scope creep, verification gaps. Fix what you find; do not report findings you could have fixed.
Commit in ${wave.worktree} with pathspec commits and a conventional subject.
${RULES}
Return the change result: status, commits (hashes you created), verified (what you ran and saw pass), notVerified, escalation (NEEDS-PLANNER text or null).`

const reviewPrompt = (wave, t, commits, cycle) => `ROLE: Reviewer, cycle ${cycle}. You are independent of the implementer. You modify nothing and write no task status; you may run tests and typechecks in the worktree.
${header(wave, t)}
Commits under review: ${commits.join(', ')} in ${wave.worktree}. Read the actual diff (git -C ${wave.worktree} show <hash>) and the surrounding code, not the implementer's summary.
Three axes: (a) correctness: root cause fixed, nothing suppressed or fabricated, tests really assert the verification requirements; (b) scope creep: anything beyond the ticket is a finding even if the code is good; (c) verification gaps: re-run the focused checks yourself where feasible, and check the implementer's notVerified first; every claim that does not hold up is a finding.
Verdict pass only with zero blocker and zero major findings; minor findings are listed, not failing.
${RULES}
Return the review result: status, verdict, findings [{id like F1, severity, summary, evidence (file:line or command output), remediation}], verified, notVerified, escalation.`

const verifyPrompt = (wave, t, commits, review) => `ROLE: Verifier. The review failed. Adversarially confirm or dismiss every finding with evidence, then write the minimal correction plan. Read-only: modify nothing, commit nothing.
${header(wave, t)}
Commits under review: ${commits.join(', ')} in ${wave.worktree}.
Review findings: ${JSON.stringify(review.findings)}
For each finding, try to refute it against the code and by running the relevant test. disposition: confirmed (real), dismissed (not real, or outside the ticket; say why), modified (real but the remediation is wrong; give the right one). correctionPlan: the smallest ordered list of concrete edits that resolves every confirmed or modified blocker and major finding, nothing else.
${RULES}
Return the verification result: status, findings [{id, disposition, evidence, correction}], correctionPlan, verified, notVerified, escalation.`

const fixPrompt = (wave, t, commits, verification) => `ROLE: Fixer. Apply the verified correction plan exactly, nothing more: no dismissed findings, no refactoring, no wider scope.
${header(wave, t)}
Existing commits: ${commits.join(', ')} in ${wave.worktree}. Add new commits on top; never amend or rewrite.
Correction plan (ordered): ${JSON.stringify(verification.correctionPlan)}
Verifier findings for context: ${JSON.stringify(verification.findings)}
Run the focused checks after the change. Commit with pathspec.
${RULES}
Return the change result: status, commits (new hashes only), verified, notVerified, escalation.`

async function runTicket(waveIn, t) {
  const wave = t.worktree ? Object.assign({}, waveIn, { worktree: t.worktree }) : waveIn
  const r = { id: t.id, area: wave.area, worktree: wave.worktree, outcome: 'unknown', commits: [], cycles: 0, reviews: [], deferred: [], notVerified: [], escalation: null }
  const take = (x) => { r.notVerified.push(...x.notVerified); if (x.escalation) r.deferred.push(x.escalation) }
  let commits = Array.isArray(t.commits) ? t.commits.slice() : []
  if ((t.mode || 'implement') === 'implement') {
    const impl = await agent(implementPrompt(wave, t), { label: `impl:${t.id}`, phase: 'Implement', schema: CHANGE, ...TIER.implement })
    if (!impl) { r.outcome = 'agent-lost'; return r }
    take(impl)
    if (impl.status !== 'completed' || impl.commits.length === 0) { r.outcome = impl.status === 'blocked' ? 'blocked' : 'implement-failed'; r.escalation = impl.escalation; return r }
    commits.push(...impl.commits)
  }
  if (commits.length === 0) { r.outcome = 'no-commits'; return r }
  r.commits = commits.slice()
  const first = (t.priorCycles || 0) + 1
  for (let cycle = first; cycle <= MAX_CYCLES; cycle++) {
    r.cycles = cycle
    const seeded = cycle === first && t.seedReview
    const review = seeded ? t.seedReview : await agent(reviewPrompt(wave, t, commits, cycle), { label: `review:${t.id}#${cycle}`, phase: 'Review', schema: REVIEW, ...TIER.review })
    if (!review) { r.outcome = 'agent-lost'; return r }
    r.reviews.push(review); take(review)
    if (review.verdict === 'unknown' || review.status !== 'completed') { r.outcome = 'review-inconclusive'; return r }
    if (review.verdict === 'pass' || review.findings.every(f => f.severity === 'minor')) { r.outcome = 'pass'; r.commits = commits.slice(); return r }
    if (cycle === MAX_CYCLES) { r.outcome = 'cap-reached'; r.escalation = `review failed ${MAX_CYCLES} times; surface to the user`; return r }
    const verification = await agent(verifyPrompt(wave, t, commits, review), { label: `verify:${t.id}#${cycle}`, phase: 'Verify', schema: VERIFY, ...TIER.verify })
    if (!verification) { r.outcome = 'agent-lost'; return r }
    take(verification)
    if (verification.findings.every(f => f.disposition === 'dismissed') || verification.correctionPlan.length === 0) { r.outcome = 'pass-after-verify'; r.commits = commits.slice(); return r }
    const fix = await agent(fixPrompt(wave, t, commits, verification), { label: `fix:${t.id}#${cycle}`, phase: 'Fix', schema: CHANGE, ...TIER.fix })
    if (!fix) { r.outcome = 'agent-lost'; return r }
    take(fix)
    if (fix.status !== 'completed' || fix.commits.length === 0) { r.outcome = 'fix-failed'; r.escalation = fix.escalation; return r }
    commits.push(...fix.commits); r.commits = commits.slice()
  }
  r.outcome = 'cap-reached'
  return r
}

async function runWave(wave) {
  const out = []
  log(`wave ${wave.area}: ${wave.tickets.length} ticket(s) in ${wave.worktree}`)
  for (const t of wave.tickets) {
    const r = await runTicket(wave, t)
    log(`wave ${wave.area}: ${t.id} -> ${r.outcome} (${r.commits.length} commit(s), ${r.cycles} cycle(s))`)
    out.push(r)
    if (!r.outcome.startsWith('pass') && wave.stopOnFailure) { log(`wave ${wave.area}: stopping after ${t.id}`); break }
  }
  return { area: wave.area, worktree: wave.worktree, tickets: out }
}

// Waves run concurrently (disjoint areas); tickets inside a wave run one at a time.
const waves = await pipeline(WAVES, w => runWave(w))
return { branch: BRANCH, waves: waves.filter(Boolean) }
