# Running the wave pipeline on Codex

How the harness-neutral roles and protocol in SKILL.md map onto the Codex CLI.

## Models and effort

Apply the tier rule to the current Codex lineup; set the model override per spawned subagent and the effort per role. As of 2026 a good mapping:

- **Planner (gpt-5.6-sol, medium effort)**
- **Implementer (gpt-5.6-terra, high effort)**
- **Reviewer (gpt-5.6-sol, high effort)**
- **Verifier (gpt-5.6-sol, xhigh effort)**
- **Fixer (gpt-5.6-terra, high effort)**

When lineups change, re-derive from the tier rule instead of copying these names.

## Spawning subagents

Spawn one subagent per role per ticket (`spawn_agent`): an implementer and then a separate reviewer for each ticket, plus a verifier and fixer only when review fails; resume them for later phases (`followup_task`), let implementer and reviewer talk (`send_message`), and close them when the ticket closes (`close_agent`). Set the model and effort override per role. Make every prompt self-contained: name the role; provide the ticket id and instruct the agent to read it with `nitro agent tasks show <id> --output json` (comments included); state the wave boundary ("touch only `<area>`"); provide the implementation commit hashes or exact diff when applicable; repeat the standing git rules from SKILL.md verbatim; and require the applicable [change](../assets/change-result.schema.json), [review](../assets/review-result.schema.json), or [verification](../assets/verification-result.schema.json) result shape. Never rely on conversation context reaching the subagent—the prompt and tracker are its contract.

Worktree isolation is native: give each implementer its own worktree when waves run concurrently. Nitro resolves the same agent workspace from every worktree of the repository, so no extra setup is needed.

## Wake integration

Install nitro's Codex hook and notify entries:

```bash
nitro agent hooks codex install      # wires the turn-boundary hook and wraps the notify program
nitro agent hooks codex status
```

With hooks installed, Nitro gives the session an actor name and states it in your context. The session appears in `nitro agent list`, so mail addressed to it can wake it, and unread mail is shown to you at the start of each turn. Unread mail also blocks you from ending a turn, up to three times, before it lets you finish. Hooks take effect from the next session, so install before the run, not mid-wave. When no actor name reaches the context, `nitro agent login` allocates one and you pass it as `--actor <name>` exactly the same way. Read hook command results from the human output and exit codes, not `--output json`.

Hooks or not, the rhythm stands: drain `nitro agent mail inbox --unread --actor <name>` between waves. The digest is a wake-up, not the inbox.
