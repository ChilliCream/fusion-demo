export const meta = {
  name: 'nitro-research',
  description: 'Answer a research brief from primary sources: scout, parallel readers, synthesizer, optional verifier',
  whenToUse: 'The research lead runs this once per job after writing brief.md. args: { folder (abs path holding brief.md), briefs (abs path to the skill\'s references/briefs.md), size: "lookup"|"question"|"investigation", source? (lookup only), maxSources? }. Agents write files into folder and never touch nitro.',
  phases: [
    { title: 'Scout', detail: 'find primary sources for the brief' },
    { title: 'Read', detail: 'one reader per source, verbatim quotes with locations' },
    { title: 'Synthesize', detail: 'findings.md: answer, evidence, conflicts, unknowns' },
    { title: 'Verify', detail: 'investigations only: re-open cited sources for load-bearing claims' },
  ],
}

const STR_ARR = { type: 'array', items: { type: 'string' } }
const SCOUT = { type: 'object', additionalProperties: false, required: ['sources', 'uncovered'],
  properties: { sources: { type: 'array', items: { type: 'object', additionalProperties: false, required: ['source', 'slug', 'covers'],
    properties: { source: { type: 'string' }, slug: { type: 'string' }, covers: STR_ARR } } }, uncovered: STR_ARR } }
const READ = { type: 'object', additionalProperties: false, required: ['reachable', 'note', 'covered', 'uncovered', 'located'],
  properties: { reachable: { type: 'boolean' }, note: { type: 'string' }, covered: STR_ARR, uncovered: STR_ARR, located: { type: 'boolean' } } }
const SYNTH = { type: 'object', additionalProperties: false, required: ['answer', 'unknowns'], properties: { answer: { type: 'string' }, unknowns: STR_ARR } }
const VERIFY = { type: 'object', additionalProperties: false, required: ['checked', 'confirmed', 'corrected', 'rejected'],
  properties: { checked: { type: 'integer' }, confirmed: { type: 'integer' }, corrected: { type: 'integer' }, rejected: { type: 'integer' } } }

// Role tiers (see references/claude-code.md). Re-derive when lineups change.
const TIER = {
  scout: { model: 'sonnet', effort: 'medium' },
  read: { model: 'sonnet', effort: 'low' },
  reread: { model: 'sonnet', effort: 'medium' },
  synth: { model: 'sonnet', effort: 'medium' },
  verify: { model: 'opus', effort: 'medium' },
}

const A = args || {}
const FOLDER = A.folder, BRIEFS = A.briefs, SIZE = A.size || 'question'
if (!FOLDER || !BRIEFS) throw new Error('args.folder and args.briefs are required')
const MAX_SOURCES = A.maxSources || 8
const COMMON = `Research folder: ${FOLDER}. Read ${FOLDER}/brief.md first, and ${BRIEFS} for the file templates and your role's contract. Never run nitro commands; the lead owns the tracker. Return only the structured result.`

const scoutPrompt = () => `ROLE: Scout. ${COMMON}
Find primary sources (official docs, source code, specs, RFCs, first-party APIs) for each sub-question; skim only, extract nothing. Secondary sources go under Rejected with the primary source they point to. Write ${FOLDER}/sources.md; at most ${MAX_SOURCES} sources, each with a kebab-case slug.`
const readPrompt = (src, n) => `ROLE: Reader. ${COMMON}
Your single source: ${src.source}. Open it and record every fact bearing on a sub-question as a claim with a verbatim quote and an exact location (URL fragment, file path and line, or section heading). Never paraphrase in place of quoting; consult no other source. Write ${FOLDER}/notes/${String(n).padStart(2, '0')}-${src.slug}.md. Set located=false if any claim lacks a location, reachable=false if the source could not be opened.`
const synthPrompt = () => `ROLE: Synthesizer. ${COMMON}
Read every file under ${FOLDER}/notes/ and write ${FOLDER}/findings.md: answer first, every claim citing a note and location, conflicts named, gaps under Unknown. Fetch nothing; fill no gap from your own knowledge.`
const verifyPrompt = () => `ROLE: Verifier. ${COMMON}
Read ${FOLDER}/findings.md and the notes it cites. For each claim the Answer depends on, re-open the cited source at the cited location and confirm it. Mark "Verified: confirmed" or rewrite and mark "Verified: corrected"; move unsupported claims to Unknown; add a "## Verification" section. Edit findings.md in place.`

phase('Scout')
let sources
if (SIZE === 'lookup') {
  if (!A.source) throw new Error('lookup needs args.source')
  sources = [{ source: A.source, slug: 'source', covers: [] }]
} else {
  const scout = await agent(scoutPrompt(), { label: 'scout', phase: 'Scout', schema: SCOUT, ...TIER.scout })
  if (!scout) return { outcome: 'agent-lost', stage: 'scout' }
  sources = scout.sources.slice(0, MAX_SOURCES)
  if (scout.uncovered.length) log(`scout: no source for ${scout.uncovered.join(', ')}`)
}

const reads = await parallel(sources.map((src, i) => async () => {
  let r = await agent(readPrompt(src, i + 1), { label: `read:${src.slug}`, phase: 'Read', schema: READ, ...TIER.read })
  if (r && r.reachable && !r.located) r = await agent(readPrompt(src, i + 1), { label: `reread:${src.slug}`, phase: 'Read', schema: READ, ...TIER.reread })
  return { source: src.source, ...(r || { reachable: false, note: '', covered: [], uncovered: [], located: false }) }
}))
const unreachable = reads.filter(r => !r.reachable).map(r => r.source)
if (unreachable.length) log(`unreachable: ${unreachable.join(', ')}`)
if (SIZE === 'lookup') return { outcome: 'read', reads }

const synth = await agent(synthPrompt(), { label: 'synthesize', phase: 'Synthesize', schema: SYNTH, ...TIER.synth })
if (!synth) return { outcome: 'agent-lost', stage: 'synthesize', reads }
let verification = null
if (SIZE === 'investigation') verification = await agent(verifyPrompt(), { label: 'verify', phase: 'Verify', schema: VERIFY, ...TIER.verify })
return { outcome: 'done', findings: `${FOLDER}/findings.md`, answer: synth.answer, unknowns: synth.unknowns, reads, verification }
