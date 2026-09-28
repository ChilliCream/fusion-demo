---
name: nitro-task
description: >-
  Create and manage tasks with `nitro agent tasks`, the local-first,
  dependency-aware issue tracker for coding agents. Use when creating or
  triaging tasks, wiring dependencies, finding ready work, or updating and
  closing what you worked on.
---

# nitro agent tasks

The tracker is the single source of truth for work and decisions, because sessions die and context compacts while tasks survive. The tracker never runs git commands and never writes anything to the working tree, so none of its state can land in a commit. Always call a subcommand, since bare `nitro agent` opens an interactive TUI and blocks the session. If `nitro` itself is not found, the CLI is not installed — stop and tell the user to install it: https://chillicream.com/docs/nitro/cli/installation. Do not attempt to install it yourself.

## Core principles

- **Use `--output json` and take ids from results.** Ids are never constructed. List commands return compact snapshots; `nitro agent tasks show <id> --output json` returns full state -- description, comments, dependencies, blockers.
- **Comments are the decision log.** User rulings and binding decisions go in `comment add`, not only in descriptions; agents read them through `show`.
- **Pass your actor name on every write**: `--actor <name>` on `create`, `q`, `update`, `close`, `reopen`, `delete`, `defer`, `undefer`, `dep add`, `dep remove`, `comment add`, `label add`, `label remove`, and `epic close-eligible`. Read commands take no actor. The name is handed to you, never invented. With Nitro's hooks installed, the session-start hook states it in your context: `Your Nitro actor name is "maya".` Otherwise `nitro agent login` allocates one and prints it.
- **Close with evidence.** `close --reason` names the commits or facts that prove it; when unsure, comment instead of closing.

## Work a task

```bash
nitro agent tasks ready --output json                        # open, unblocked, unclaimed work
nitro agent tasks show "app-1a2" --output json               # read the full task, comments included
nitro agent tasks update "app-1a2" --claim --actor maya  # in_progress + assigned to you
# ...work...
nitro agent tasks close "app-1a2" --actor maya --reason "Implemented X in commit abc123"
```

`--claim` does not refuse a ticket someone else holds -- check `show` first; `in_progress` with another assignee means pick different work.

## Create

```bash
nitro agent tasks create "Fix the parser" --actor maya \
  --type bug --priority p1 --label parser \
  --description "..." --depends-on "app-9z8" --parent "app-3f2"
nitro agent tasks q "Quick capture" --actor maya         # prints only the new id
```

- Priorities are 0-4 / p0-p4 (0 critical, 2 default, 4 backlog); types: `task`, `bug`, `feature`, `epic`, `chore`, `docs`, `question`, or custom.
- `--parent <epic>` makes the task a child with an id like `<epic>.<n>`; write a real description -- `lint` flags empty ones.

## Query

```bash
nitro agent tasks ready --label api --include-deferred --output json
nitro agent tasks blocked --output json                      # what is stuck and on what
nitro agent tasks list --status open --status in_progress --output json
nitro agent tasks search "parser" --output json              # full text, comments included
nitro agent tasks stale --days 14 --output json
nitro agent tasks count --by status --output json
```

- `list` shows non-terminal work by default; `--all` adds closed tasks. Beyond 100 closed tasks the oldest closes are auto-archived and hidden from `list --all` and `search` -- reach them with `list --status archived` or `show <id>`. Do not treat a missing search hit as proof a task never existed.
- Default ordering is by priority; there is no `--sort`.

## Dependencies and epics

```bash
nitro agent tasks dep add "app-1a2" "app-9z8" --actor maya   # app-1a2 depends on app-9z8 (blocks)
nitro agent tasks dep tree "app-1a2" --output json
nitro agent tasks dep cycles --output json                       # must return empty
nitro agent tasks epic status --output json                      # child completion per epic
nitro agent tasks epic close-eligible --actor maya           # closes every epic whose children are all closed
```

- A `blocks` edge gates `ready`: a task is unblocked when everything it depends on is closed.
- A parent edge (`--parent`) blocks only the epic, never the child -- children stay schedulable while the epic waits for them. Never link a child to its epic with bare `dep add`; that blocks the child on an epic that cannot close first, and `dep cycles` will not catch it.
- `epic close-eligible` closes *every* eligible epic; when some epics belong to other workflows (e.g. `wayfinder:*` planning maps), close epics individually via `epic status` instead.

## Defer and housekeeping

```bash
nitro agent tasks defer "app-1a2" --until "2026-09-01" --actor maya   # hide from ready until then
nitro agent tasks undefer "app-1a2" --actor maya
nitro agent tasks label add "app-1a2" api parser --actor maya
nitro agent tasks comment add "app-1a2" "Ruling: keep the old format." --actor maya
nitro agent tasks lint --output json                          # quality findings (e.g. empty descriptions)
nitro agent tasks doctor --output json                        # workspace integrity
```

Prefer `close --reason` over `delete` -- close keeps the audit trail; a non-interactive `delete` requires `--force`.
