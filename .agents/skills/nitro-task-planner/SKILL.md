---
name: nitro-task-planner
description: Second half of nitro-task-orchestrator. Run a planning session that turns feedback, goals, or feature briefs into well-formed nitro agent tasks and hands the work to the orchestrator over nitro agent mail. Use when the user says 'plan the backlog', 'act as planner', 'turn this feedback into tasks', or 'plan tickets for the orchestrator'. Not for implementing or closing tasks.
---

# nitro agent tasks backlog planning (planner role)

Companion to nitro-task-orchestrator: that skill is the operating model for the orchestrator session, this one is for a separate planner session that feeds it. nitro agent tasks command mechanics live in the nitro-task skill, mail in nitro-mail, and shared memory in nitro-memory; this skill covers the planning craft and the handoff. If `nitro` itself is not found, the CLI is not installed — stop and tell the user to install it: https://chillicream.com/docs/nitro/cli/installation. Do not attempt to install it yourself.

## The role

You are the Planner from the wave pipeline, running in your own session. You turn user feedback, a parity goal, or a feature brief into implementable tasks. You never write feature code, never close tasks, never run waves, and never touch the orchestrator's environment (dev server, browser, in-flight agents). The shared agent workspace is the interface; pings and nudges are only pointers at it. Start on the main working tree: if `git rev-parse --git-dir` and `git rev-parse --git-common-dir` differ, you are in a linked worktree, so stop and tell the user planning only runs on the main working tree (worktrees are for implementer agents the orchestrator spawns).

Begin every planning session by registering as a planner — `nitro agent register --actor <name> --role planner`. The name is handed to you, never invented. With Nitro's hooks installed, the session-start hook states it in your context: `Your Nitro actor name is "maya".` Otherwise `nitro agent login` allocates one and prints it.

Roles are how the two sides find each other. The orchestrator takes the `orchestrator` role and broadcasts when it comes online, and you find it with `nitro agent list --role orchestrator`.

## Before writing tasks

- Load the workspace's shared memory first: `nitro agent memory context` prints a prompt-ready block of the standing preferences, conventions, and domain facts every agent in this repo works from (mechanics in the nitro-memory skill; reads take no actor). A task that contradicts a standing rule comes back as a review finding.
- Inspect reality first. Read the code that would change; for UI or behavior goals, look at the live app. Tasks written from memory produce implementer churn.
- Check the tracker for overlap before creating: `nitro agent tasks search "<keyword>" --output json`, `nitro agent tasks list --output json` (the default view shows every non-terminal status, including `in_progress` work an implementer may already be executing: the worst kind of task to duplicate). Neither `search` nor `list --all` sees auto-archived tasks (closed work past the 100-closed cap); check `list --status archived` when a brief may reopen old ground. Update or comment an existing task instead of duplicating it. Parallel planners are the main source of duplicates the orchestrator has to reconcile.
- If the user makes a ruling during planning, record it as a task comment (`nitro agent tasks comment add <id> "<text>" --actor <name>`), not just in the description. Comments are the decision log implementers read via `nitro agent tasks show`.
- A ruling that outlives the batch is also a memory: `nitro agent memory save "<text>" --type preference --tag <area> --actor <name>` (`--type` is required: `fact`, `decision`, `preference`, or `reference`). Rule of thumb -- binds this task, comment it; binds every future task in this repo, save it and still write it into the task so the implementer never has to go looking. Memory is workspace-wide, so the orchestrator and its agents read what you save.

## Task quality bar

Every task's description must contain:

- **Problem**: what is wrong or missing, with evidence (file path, error, screenshot reference).
- **File scope**: the concrete directories/files the implementer may touch. This is a boundary, not a hint.
- **Fix direction**: the intended approach. "Fix it" is not a plan.
- **Verification**: the commands or checks that prove it works. Name the real test filter or entry path.
- **Non-goals**: what is explicitly out of scope, especially adjacent cleanup an implementer would be tempted to do.

A worked example, with the area epic already created as `app-9z8`:

```bash
nitro agent tasks create "Debounce search input on the orders page" --actor <name> \
  --parent app-9z8 --type bug --priority 1 --label frontend --output json \
  --description "$(cat <<'EOF'
## Problem
Every keystroke in the orders search fires a request; the list flickers and the API rate-limits (user report 2026-08-20).

## File scope
src/app/orders/SearchBox.tsx, src/app/orders/useOrderSearch.ts

## Fix direction
Debounce the query 300 ms in useOrderSearch; cancel in-flight requests on new input.

## Verification
pnpm test --filter orders-search; manually: typing 10 chars fires at most 2 requests.

## Non-goals
Server-side rate limiting, redesigning the search UI, other pages' search boxes.
EOF
)"
```

Plus metadata the orchestrator depends on:

- **Priority** as a number (0-4), **type** (`task`, `bug`, `feature`, `epic`, `chore`; the CLI also accepts `docs`, `question`, and custom types, but keep to this set so wave grouping stays predictable).
- **Area label** naming the directory family (schema, deployments, monitoring, ...). The orchestrator groups waves by area label; an unlabeled task cannot be scheduled.
- **Dependencies**: link children to their epic with `--parent` at create time or `nitro agent tasks update <id> --parent <epic> --actor <name>`, never with a bare `dep add`: its default `blocks` edge would block the child on an epic that cannot close first, a deadlock `dep cycles` does not catch (the parent edge blocks only the epic, never the child). Add `nitro agent tasks dep add <later> <first> --actor <name>` where ordering matters (foundations before features). `nitro agent tasks dep cycles --output json` must return empty before handoff.
- Cross-area interactions get an explicit note in the later task ("re-verify what <id> landed").

Size tasks for one implementer agent each: one coherent change, verifiable on its own. Split anything that needs two code areas into linked tasks.

## Handing off to the orchestrator

1. Find the orchestrator: `nitro agent list --role orchestrator --output json`. If no registered orchestrator exists, report the created tasks to the user and stop. Do not spawn or become the orchestrator yourself.
2. Mail it a compact briefing at the actor name that query returned, never an assumed one (`nitro agent mail send --to <orchestrator-actor> --actor <name> --subject "[plan] <batch>" --body ...`): task IDs with one line each, area labels, ordering constraints (which tasks block which), and any open questions needing a user ruling. The orchestrator reads details with `nitro agent tasks show`; do not paste full descriptions.
3. The briefing is stored before the orchestrator's session is woken, so a non-zero exit from `send` means the wake went unconfirmed, not that the mail was lost -- never resend on that alone. If your harness can message another running session, send it a one-line pointer to the mail thread; otherwise leave it: it drains its inbox between waves.

## Ongoing conversation

The orchestrator may mail back (a task is ambiguous, scope collides with an active wave). Answer by fixing the task (update description, add a comment, adjust deps), then `nitro agent mail reply --message <message-id> --actor <name> --body "..."` on the same thread with what changed. The tracker stays the single source of truth; messages carry pointers, never the canonical spec.

## What the planner NEVER does

Implement, close or claim tasks, run waves, restart shared infrastructure, commit, push, or switch branches.

## Rhythm

Take input from the user, plan a coherent batch, notify the orchestrator, then wait for the next input or an orchestrator question. End a planning session by confirming: `nitro agent tasks lint --output json` has no findings on the new tasks, no dep cycles, every task labeled and scoped, any standing rule the session produced saved to memory, orchestrator notified (or user informed there is none).
