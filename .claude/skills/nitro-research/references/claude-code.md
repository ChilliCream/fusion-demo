# Running research on Claude Code

How the harness-neutral roles in SKILL.md map onto Claude Code.

## Models and effort

The tier rule: locating and extracting get the cheapest model that quotes accurately; judging gets a mid tier; only adversarial verification gets a top tier, and only for investigations. As of 2026 a good mapping:

- **Lead**: whatever the session runs; it reads three short files and writes one paragraph, so its cost is the brief, not the model.
- **Scout**: sonnet, medium effort; one run decides what every reader sees, and its cost is search tokens, not reasoning
- **Reader**: sonnet, low effort; a reader whose claims lack locations reruns at medium effort.
- **Synthesizer**: sonnet, medium effort
- **Verifier**: opus, medium effort; fable only when the caller says the decision is expensive to get wrong.

When lineups change, re-derive from the tier rule instead of copying these names; the mapping lives in the `TIER` table of the workflow script.

Haiku is not in the mapping on purpose: it costs half of Sonnet on the cheapest roles, is a generation older, has a 200K context that dense sources overflow, and a reader that quotes the wrong location costs more in reruns than it saved. Lower effort on the newer model is the cheaper lever.

## The pipeline is a workflow

After step 3 (brief.md written) run the Workflow script in [assets/workflows/nitro-research.js](../assets/workflows/nitro-research.js): scout, readers in parallel, synthesizer, verifier for investigations, each with the model and effort above, a failed reader re-run one tier up. Install it once (`cp` into `.claude/workflows/`), then:

```
Workflow({ name: "nitro-research", args: { folder: "<abs .nitro/research/...>", briefs: "<abs path to references/briefs.md>", size: "question" } })
```

Agents write into the folder and never touch the tracker; the result carries the answer, unknowns, per-source read summaries, and the verification counts. Create, comment, and close the step tasks from that result. A killed run resumes with `resumeFromRunId`.

## Worktrees

When research must stay off the main branch, give the lead a worktree on `research/<slug>` (Agent tool `isolation: "worktree"` for the readers is unnecessary; they only write into the research folder). Commit the folder once at close.
