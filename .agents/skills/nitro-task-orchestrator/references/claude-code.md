# Running the wave pipeline on Claude Code

How the harness-neutral roles and protocol in SKILL.md map onto Claude Code.

## Models and effort

Apply the tier rule to the current Claude lineup. As of 2026 a good mapping:

- **Planner (fable, medium effort)**: usually a separate session running the nitro-task-planner skill, not a subagent you spawn.
- **Implementer (sonnet, high effort)**
- **Reviewer (opus, medium effort)**
- **Verifier (fable, medium effort)**
- **Fixer (sonnet, high effort)**

When lineups change, re-derive from the tier rule instead of copying these names; the mapping lives in the `TIER` table of the workflow script.

## The wave is a workflow

A wave runs as the Workflow script in [assets/workflows/nitro-backlog-wave.js](../assets/workflows/nitro-backlog-wave.js): deterministic implement, review, verify, fix loop, model and effort per role, result schemas enforced, killed runs resumable with `resumeFromRunId`. Install it once (`cp` into `.claude/workflows/`), then per wave, after claiming tickets and creating the worktree:

```
Workflow({ name: "nitro-backlog-wave", args: { actor, repo, branch, rules?, tooling?,
  waves: [{ area, worktree, tickets: [{ id, context? }] }] } })
```

Each phase is a fresh agent; the review result carries the findings forward. Disjoint areas may share one call; tickets inside a wave run one at a time. The result carries per ticket the outcome, commits, cycles, and deferred notes: close the passes with their commits as evidence, escalate the rest. Standing rules from memory go into `args.rules`.

Worktree isolation is native: give each wave its own worktree when waves run concurrently. Nitro resolves the same agent workspace from every worktree of the repository, so no extra setup is needed.

## Wake integration

Install nitro's turn-boundary hooks so mail reaches sessions without polling:

```bash
nitro agent hooks claude install --scope user  # or --scope project
nitro agent hooks claude status
```

With hooks installed, Nitro gives the session an actor name and states it in your context. The session appears in `nitro agent list`, so mail addressed to it can wake it, and unread mail is shown to you at the start of each turn. Unread mail also blocks you from ending a turn, up to three times, before it lets you finish. When no actor name reaches the context, `nitro agent login` allocates one and you pass it as `--actor <name>` exactly the same way. Read hook command results from the human output and exit codes, not `--output json`.

Hooks or not, the rhythm stands: drain `nitro agent mail inbox --unread --actor <name>` between waves. The digest is a wake-up, not the inbox.
