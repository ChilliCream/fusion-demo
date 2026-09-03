# Research tickets: facts a decision waits on

A research ticket surfaces a fact from outside the working directory: vendor documentation, a third-party API, a standard, a knowledge base. It is AFK: the nitro-research skill resolves it with background subagents while you keep working, and it is the one ticket type you may resolve several of in one session.

## When to create one

Create a research ticket when a grilling question would otherwise ask the user for something they would have to look up, and other tickets will depend on the answer. Make it a real ticket, not a side note: the `blocks` edge is what renders that dependency in the frontier. A fact only the current ticket needs is looked up inline by a subagent and recorded in that ticket's resolution comment instead.

Facts that come from the codebase itself are not research tickets; look them up in-session.

## Dispatching

Claim the ticket first (`update <id> --claim`), then run the nitro-research skill yourself: you are its lead. The lead never reads a source and only reads three short files back, so it costs the session little; the scout, readers, and synthesizer run as background subagents while you resolve the one decision by hand. Give nitro-research the ticket's question verbatim, name this ticket as the caller in the research epic it opens, and size it `question` by default, `investigation` when several tickets hang on the answer. It leaves a cited `findings.md` under `.nitro/research/<epic-id>-<slug>/` and saves the answer to memory.

Do this for every research ticket at once. While charting, open all research epics and dispatch their scouts right after wiring edges. While working the map, do it for every research ticket on the frontier at the start of the session, before the decision; check on the pipelines at step boundaries and record each resolution as its findings land.

If the repository wants research off the main branch, run the research lead in a separate worktree on a `research/<slug>` branch (see Git rules in operations.md) and name the branch in the resolution.

## Resolving

When the research pipeline closes:

```bash
nitro agent tasks comment add bill-3f2.2 --actor maya "$(cat <<'EOF'
## Decision
Exports land in the existing finance share; the reconciliation job already mounts it.

## Rejected
- New bucket: finance has no credentials for it and would need a second tool.

## Locked parameters
- path: /finance/exports/invoices/

## Assets
- .nitro/research/rs-2b9-export-storage/findings.md
EOF
)"
nitro agent tasks close bill-3f2.2 --actor maya --reason "Researched: finance share, path locked"
```

The comment must carry the answer, not only a pointer. A future session reads the ticket first and the file only if it needs the evidence. Then append the gist to the map's **Decisions so far** and run the graduation pass: a fact often turns a fog entry into a sharp question.

## Guardrails

- A pipeline that could not reach a primary source says so in its findings, and the ticket stays open with a comment saying what is missing. Never close on a guess.
- Research resolves facts, not decisions. If the finding forces a choice, the choice is a grilling ticket.
