# Running research on Codex

How the harness-neutral roles in SKILL.md map onto the Codex CLI.

## Models and effort

The tier rule: locating and extracting get the cheapest model that quotes accurately; judging gets a mid tier; only adversarial verification gets a top tier, and only for investigations. As of 2026 a good mapping:

- **Lead**: whatever the session runs; it reads three short files and writes one paragraph, so its cost is the brief, not the model.
- **Scout (gpt-5.6-terra, medium effort)**; one run decides what every reader sees, and its cost is search tokens, not reasoning
- **Reader (gpt-5.6-terra, low effort)**; rerun a failed reader on gpt-5.6-terra, medium effort, then gpt-5.6-sol.
- **Synthesizer (gpt-5.6-terra, medium effort)**
- **Verifier (gpt-5.6-sol, high effort)**

When lineups change, re-derive from the tier rule instead of copying these names. Prefer lower effort on the newer model over an older, smaller model: a reader that quotes the wrong location costs more in reruns than it saved.

## Spawning

Launch a fresh subagent for every role invocation and pass the model and reasoning effort explicitly on each spawn; Codex honors both per invocation, so no agent definitions are needed. Never share the lead's conversation with a subagent: it would inherit the whole context, which is exactly the cost this pipeline avoids.

Every prompt is self-contained: name the role, give the absolute folder path and the file the agent must write, paste the role's contract from [briefs.md](briefs.md), and end with "Return at most ten lines: what you wrote, where, and any failure." Readers get one source each and run in parallel. Do not give subagents your actor name or any `nitro` command; the lead claims, comments, and closes every step task itself from the subagent's returned summary. Nothing from the lead's conversation reaches a subagent unless it is in the prompt or in the folder.

Readers need web access for web sources and file access for repository sources. Scouts need web search and fetch. Synthesizer and verifier need only file access; the verifier additionally fetches the sources it re-checks. Grant network access to the subagents that need it; a sandboxed reader that cannot reach its source must report that, never guess.

## Worktrees

When research must stay off the main branch, run the lead in a worktree on `research/<slug>`; readers only write into the research folder and need no isolation of their own. Nitro resolves the same agent workspace from every worktree of the repository. Commit the folder once at close.
