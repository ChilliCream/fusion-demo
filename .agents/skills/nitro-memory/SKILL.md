---
name: nitro-memory
description: >-
  Save and recall shared, durable agent memory with `nitro agent memory`:
  knowledge that must outlive the session -- preferences, decisions, domain
  facts, references -- readable by every agent in the workspace. Use when the
  user says "remember this", when a session should start from what the
  workspace already knows, or when knowledge worth keeping surfaces mid-task.
---

# nitro agent memory

Memory is the workspace's shared knowledge: what one session learns, every future session and every other agent starts from. Read it at session start, capture cheaply while you work, curate what proves durable. It lives locally with the repository, shared across all its git worktrees, never pushed to the remote. Always call a subcommand; bare `nitro agent` opens an interactive TUI and blocks the session. If commands report no workspace, run `nitro agent init` once from the repository root (see the sibling `nitro-task` skill). If `nitro` itself is not found, the CLI is not installed — stop and tell the user to install it: https://chillicream.com/docs/nitro/cli/installation. Do not attempt to install it yourself.

## Core principles

- **Memory holds what you would otherwise repeat in every prompt.** Standing preferences, durable domain facts, references. Work and decisions in flight live in `nitro agent tasks`; messages to agents go over `nitro agent mail`; follow-ups and TODOs go to the tracker.
- **Every agent reads everything.** Reads are not filtered by actor. Writes record who saved it: pass `--actor <name>` on `save` and `log`, the only two memory commands that take an actor. The name is handed to you, never invented. With Nitro's hooks installed, the session-start hook states it in your context: `Your Nitro actor name is "maya".` Otherwise `nitro agent login` allocates one and prints it.
- **Capture is cheap, curation is deliberate.** `log` without ceremony; `save` or `promote` only what proved durable.
- **One self-contained fact per memory.** A future session has no chat context: "Prefer X over Y because Z", not "as discussed, X".
- **Save only what every agent may read and what stays true.** Credentials, tokens, and personal data stay out (deletion is not an erasure guarantee); volatile state -- current branch, counts, progress -- belongs in the journal or the tracker.

## Recall

```bash
nitro agent memory context --tag billing --max-chars 4000   # prompt-ready block for session start
nitro agent memory search "deploy checklist" --output json
nitro agent memory recent --limit 5 --output json
nitro agent memory show "01hqzxk8..." --output json         # full curated entry with text
nitro agent memory tags --output json                       # reuse existing tags; an unfindable memory is lost
```

- `context` admits whole curated entries, newest first, until `--limit` or `--max-chars`; it never truncates an entry. Zero entries on a small budget means the newest entry alone exceeded it -- raise the budget, the store is not empty. Its plain output is the prompt block; with `--output json` it returns `{"entries": [...]}`, the one exception to the `{"items": [...]}` wrapper every other listing uses.
- `search` is whole-word AND over the entry text: every query word must appear as a whole word, any order -- no operators, no partial words. A failed search proves the words are absent, not the knowledge; retry with words the text would actually contain. Curated only by default: `--collection journal` or `all` includes the journal, and any `--tag`/`--type` filter silently drops every journal hit.

## Capture

```bash
nitro agent memory log "Investigated the flaky order test; suspect clock skew, unresolved." --actor maya
```

The journal takes text (or `--file`), no type, no tags, immutable -- and write-only through the CLI: no command prints a journal entry's text, and `show`/`update`/`forget` reject journal ids with "Memory '<id>' does not exist."

## Curate

```bash
nitro agent memory save "API JSON is camelCase; never snake_case." --type decision --tag conventions --actor maya
nitro agent memory promote                                     # list unpromoted journal candidates
nitro agent memory promote "01hqzxk8..." --type fact --tag testing
```

- `save` requires `--type`: `fact`, `decision`, `preference`, `reference`, or a custom type. Types and tags are trimmed and lowercased, then must be only lowercase letters, digits, and hyphens, up to 40 characters.
- When a closed task's decision yields a standing rule, save the outcome as a `--type decision` memory; the deliberation stays in the task.
- `promote` copies a journal entry verbatim into curated (`--type` required, records `promotedFrom`). Re-promoting returns the existing copy and ignores new flags; change them with `update`.
- Because journal text is unreadable, promote what you logged while it is still in your context. For a cold candidate: promote with a provisional `--type`, `show` the copy to read it, `forget` it if it does not earn curation (re-promoting later works).

## Housekeeping

```bash
nitro agent memory update "01hqzxk8..." --type decision --add-tag api --remove-tag draft
nitro agent memory forget "01hqzxk8..." --force              # hard delete: no tombstone, no undo
```
