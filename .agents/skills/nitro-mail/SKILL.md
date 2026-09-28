---
name: nitro-mail
description: >-
  Send and receive mail between coding agents with `nitro agent mail`:
  local-first, per-workspace messaging for multi-agent coordination. Use when
  messaging or replying to another agent, draining an inbox, or waiting for
  new mail.
---

# nitro agent mail

Mail is how agents in one workspace reach each other, with messages addressed by actor name and grouped into threads. One mailbox lives with the repository and is shared across all its git worktrees, so an agent in a linked worktree sees the same mail as one on the main tree. If `nitro` itself is not found, the CLI is not installed — stop and tell the user to install it: https://chillicream.com/docs/nitro/cli/installation. Do not attempt to install it yourself.
## Core principles

- **Mail carries pointers, never the canonical record.** Decisions and specs live in `nitro agent tasks`; a message references them. Correlate a thread to a task through the subject: `[app-1a2] Starting`.
- **You act under an actor name, and every mail command takes it: `--actor <name>`.** The name is handed to you, never invented. With Nitro's hooks installed, the session-start hook states it in your context: `Your Nitro actor name is "maya".` Otherwise `nitro agent login` allocates one and prints it.
- **Address mail to a name you looked up, never one you assumed.** `nitro agent list --role <role> --output json` names the actors this workspace knows, with their session when they have one.
- **Read state is yours alone.** Reading, acking, and archiving affect only your copy; archiving is an inbox display state, not deletion -- archived mail stays in `threads`, `search`, and `inbox --all`.

## Send and reply

```bash
nitro agent mail send --to "theo" --actor maya --subject "[app-1a2] Starting" --body "Claiming this now."
nitro agent mail send --to "theo" --to "nina" --cc "eli" --actor maya --subject "Status" --body-file notes.txt
nitro agent mail reply --message "m-abc123" --actor maya --body "On it."
nitro agent mail broadcast --actor maya --role "backend" --subject "Heads up" --body "Deploying at 5pm."
```

- Sending to a never-registered name succeeds and creates an implicit mailbox for it, with a `note: '<name>' has never registered.`; only an invalid name is a hard failure. Check the spelling when the note surprises you.
- `reply` computes recipients from the thread: the original sender plus its to/cc, minus you, all flattened into `to`. Subject is inherited. Replying to a message you neither sent nor received fails.
- `broadcast` reaches every registered agent except you (implicit ones excluded); `--role` narrows it to that role's agents.
- Sends fire a best-effort wake ping at recipients with a live claimed session -- a convenience, never a delivery guarantee.

## Receive

```bash
nitro agent mail inbox   --unread --actor maya --output json
nitro agent mail read    --message m-abc123 --thread --actor maya        # whole thread oldest first, marks it read
nitro agent mail ack     --message m-abc123 --message m-def456 --actor maya   # mark read without printing
nitro agent mail archive --message m-abc123 --actor maya                 # done with it
nitro agent mail threads --actor maya --output json                      # your threads, last activity first
nitro agent mail search  --text "deploy" --actor maya --output json      # subject, body, and sender, case-insensitive
```

- Ack what you act on, archive what is done, so other agents see the thread was handled.
- `ack` and `archive` batches are all-or-nothing: one unknown or foreign message id fails the whole batch. Archiving a message you only sent fails -- you have no recipient copy.
- `search` covers only mail you sent or received; it never surfaces another agent's private threads.

## Wait for mail

```bash
nitro agent mail watch --actor maya --timeout 30
```

`watch` blocks until mail addressed to you arrives, prints it oldest first, and exits 0; with `--timeout <s>` it exits 1 with empty stdout when nothing came. Its baseline is the moment it starts: already-unread mail does not trigger it -- drain `inbox --unread` first, or pass `--after <timestamp|id>` / `--include-existing` to have the backlog delivered. It never marks anything read; follow up with `read` or `ack`.
