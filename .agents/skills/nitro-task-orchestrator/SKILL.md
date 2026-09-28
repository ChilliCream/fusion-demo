---
name: nitro-task-orchestrator
description: Drive a nitro agent tasks backlog to completion as the orchestrator; waves of subagents grouped by code area, every task reviewed before it closes. Use when the user says 'orchestrate the backlog', 'run the wave pipeline', or wants many tasks implemented autonomously. Not for single small changes.
---

# nitro agent tasks backlog orchestration (wave pipeline)

One orchestrator, delegated workers, nitro agent tasks as the single source of truth. Command mechanics live in the nitro-task, nitro-mail, and nitro-memory skills; this skill is the operating model on top. If `nitro` itself is not found, the CLI is not installed — stop and tell the user to install it: https://chillicream.com/docs/nitro/cli/installation. Do not attempt to install it yourself.

## The roles

- **Orchestrator (you, the main session)**: never writes feature code. Owns the backlog, the schedule, the environment, and all task closures. Everything else is delegated.
- **Planner**: turns user feedback or a parity goal into tasks. Each task gets: problem, concrete file scope, fix direction, verification requirements, non-goals, priority. Planners inspect the live app and the code before writing tasks, and link epics with parent-child deps. Usually a separate session running the nitro-task-planner skill, not a subagent you spawn.
- **Implementer**: one task per agent. Reads the task with `nitro agent tasks show <id> --output json` including comments (comments carry binding decisions). Implements exactly the scope, verifies, reviews its own diff on the reviewer's three axes and fixes what it finds, commits, reports structured results.
- **Reviewer**: reads the actual diff. Three axes: correctness (root cause, not suppression), scope creep (anything beyond the task is a finding even if the code is good), verification gaps (claims that do not hold up). Verdict pass only with zero blocker/major findings.
- **Verifier**: only runs when the review fails. Adversarially confirms or dismisses each finding with evidence, then writes a minimal correction plan. This kills plausible-but-wrong findings before they cause churn.
- **Fixer**: applies the verified plan exactly, nothing more. Then re-review. Cap at 3 cycles, then surface to the user.

Roles are capabilities, not model names. For the model and effort mapping and the spawning, isolation, and wake mechanics of your harness, read [references/claude-code.md](references/claude-code.md) or [references/codex.md](references/codex.md).

## Mail identity (planner channel)

Planners and the orchestrator communicate over `nitro agent mail` (mechanics in the nitro-mail skill); this is the protocol.

1. Check for a rival first: `nitro agent list --role orchestrator --output json` names the actors this workspace knows in that role, so any live hit is another orchestrator: stop and ask the user. Only then take the role: `nitro agent register --actor <name> --role orchestrator`. `<name>` is the actor your session context states, or one from `nitro agent login`; never invent it, and repeat `--role orchestrator` on each register, since omitting it writes an empty role. Planners find you by role, so the role is the part that must be right.
2. Broadcast your existence: `nitro agent mail broadcast --actor <name> --subject "orchestrator online" --body "<workspace, branch, current wave state>"`. "No other registered agent to broadcast to." is fine at startup; later planners find you by role.
3. Planner briefings arrive as mail. Drain the inbox between waves (`nitro agent mail inbox --unread --actor <name>`, `read` what needs attention, `ack` the rest); answer on the same thread with `nitro agent mail reply --message <message-id> --actor <name> --body "..."`. Pass `--actor <name>` on every mail command and every task write. Mail carries pointers; the task itself (`nitro agent tasks show <id>`) is the spec.

## Shared memory

Knowledge that outlives this backlog lives in `nitro agent memory` (mechanics in the nitro-memory skill): one workspace-wide store every agent and session reads, so the conventions and tooling quirks an earlier run paid for are not rediscovered here.

1. Load it once at startup, before you group the first wave: `nitro agent memory context` prints a prompt-ready block; `nitro agent memory search "<words>" --output json` when a specific question smells familiar. Reads take no actor.
2. Save what will still bind after this backlog ships -- a formatter rule, a build quirk, a standing user preference: `nitro agent memory save "<text>" --type preference --tag <area> --actor <name>`. `--type` is required (`fact`, `decision`, `preference`, `reference`).
3. Per-task rulings stay task comments. Memory is for what a future run needs without reading this backlog; a comment is for what this task's implementer needs. When a closed task yields a standing rule, save the rule and leave the deliberation in the task.
4. **Subagents do not read memory unless told to.** Restate the standing rules in the implementer, reviewer, and fixer prompts yourself; a rule an agent never sees is a rule it breaks.

## The wave model

1. Group ready tasks into waves by **area label** (`nitro agent tasks ready --label <area> --output json` is one wave's worklist). Unlabeled ready tasks go back to the planner; `wayfinder:*` tasks are never wave material.
2. At wave launch, claim the wave's tickets (`nitro agent tasks update <ids...> --claim --actor <name>`); release a skipped one with `--status open --assignee ""`. Inside a wave: tickets run strictly one at a time (shared files, shared verification).
3. Across waves: run concurrently only when their areas are disjoint. State the boundary in every agent prompt ("touch only `<area>`; other waves are active elsewhere").
4. Global operations run solo, no other agents committing: schema regeneration, merging main, anything touching shared config. An in-progress git merge blocks every agent's commits, so never merge mid-wave.
5. Order waves by dependency: foundations first (routing/state, transports, schema sync), features on top, polish last. Cross-wave interactions get an explicit note in the later ticket ("re-verify what X landed").

Drive every ticket through the same implement, review, verify, fix loop, so a wave launch is just a ticket list plus per-wave rules. Agents may run in their own git worktrees; nitro resolves the same workspace from every worktree.

## Escalation modes

When you are stuck, need more input, or need something clarified, the planner is the escalation target. The planner owns the conversation with the user; never route around it to ask the user yourself. Escalate over mail: reply on the originating briefing thread when one exists, otherwise `nitro agent mail broadcast --role planner --actor <name> --subject "<task-id>: <blocker>" --body "..."`.

- **Planner responsive**: mail immediately with 2-3 concrete options and a recommendation. Record the ruling as a task comment so agents inherit it.
- **Planner away (deferred mode)**: agents pick the most conservative reasonable interpretation, implement it, record `NEEDS-PLANNER: <question> | chose: <what I did>` as a task comment, and continue. Only hard-block when no reasonable interpretation exists. The orchestrator compiles all deferred notes into the final report.
- Never let one stuck ticket stall a wave: record, skip, continue.

## Closing discipline

- Anything labelled `wayfinder:*` (planning maps and their decision tickets) belongs to planning sessions: never claim, close, or epic-close it.
- Only the orchestrator closes implementation tasks, and only after a review pass (`nitro agent tasks close <id> --actor <name> --reason ...`). Close reasons name the commits and the evidence.
- Task comments are the decision log: user rulings, attribution notes, sequencing decisions, known tooling quirks. Future agents read them via `nitro agent tasks show`.
- After an environment incident (disk full, process death), re-check recent closures with `nitro agent tasks list --status closed`: a close you issued may not have landed.

## Environment ownership

The orchestrator personally manages shared infrastructure; agents get access instructions but must never restart/steal it:
- Dev server (background, restart with a fresh cache when module resolution breaks after dependency changes).
- One authenticated browser via CDP for live verification. The user logs in interactively; agents connect read-only, never enter credentials, never log out. Poll for real session validity (a protected route loading), not cookie presence.
- When auth expires mid-run, do not churn: reviewers treat pending live checks as minor tracked gaps, and one consolidated live sweep runs at the end once the session is back.

## Rules that prevent repeat incidents

1. **Atomic commits with pathspec**: `git commit -m "..." -- <files>`, never `git add` then bare `git commit`. A bare commit sweeps other agents' staged files into your commit.
2. **Formatter before every commit** (`prettier --write` then `--check` on touched files): the CI format gate failed multiple reviews before this became a standing rule.
3. **Shared build outputs**: parallel agents build storybook to unique output dirs; never run the shared test:storybook target concurrently. Restore known side-effect files (relay.config.json) to HEAD before committing.
4. **git index contention**: on index.lock failure, sleep and retry, never force.
5. **Verify state claims yourself**: a clean incremental typecheck can be a cache hit (re-run with --force when it matters); "logged in" cookies can be server-side dead; an agent reporting "done" without a commit hash means the work may be uncommitted or swept.
6. **Agents never close tasks, never push, never switch branches, never touch the user's uncommitted files.** The user owns final git state; push only when they say so.
7. **Structured agent output**: use [change-result.schema.json](assets/change-result.schema.json) for implementers/fixers, [review-result.schema.json](assets/review-result.schema.json) for reviewers, and [verification-result.schema.json](assets/verification-result.schema.json) for verifiers. `notVerified` is mandatory honesty, and the reviewer's first job is checking it.

## Rhythm

Launch wave → process completion (close passes, fix-or-escalate failures) → record decisions → launch next wave(s). Between waves, promote any standing rule the wave produced into memory, then reconcile the tracker (`nitro agent tasks ready --output json`, `nitro agent tasks dep cycles --output json`, close each finished non-wayfinder epic via `nitro agent tasks epic status --output json`, duplicates from parallel planners) and drain the mailbox. Do not use `epic close-eligible`: it would also close a finished wayfinder map. End with: no open work (`nitro agent tasks list --output json` returns nothing after epics close), a final report of what shipped, and the compiled deferred-decisions list.
