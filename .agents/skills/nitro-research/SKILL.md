---
name: nitro-research
description: Answer a question with cited evidence from primary sources (official docs, source code, specs, first-party APIs). Use when the user says "research", or when a decision or plan waits on a fact from outside the working directory. Not for facts in the current codebase; grep for those.
---

# nitro-research

A question needs an answer that is backed by sources, not by recall. Produce a Markdown file that leads with the answer, cites every claim, and names what stayed unknown. Do it without burning the expensive model's context on raw reading: the lead session frames and judges, cheap subagents read. The steps live in `nitro agent tasks` so a dead session can be resumed, and the answer lands in `nitro agent memory` so nobody researches it twice. Command mechanics for both live in the nitro-task and nitro-memory skills; this skill covers how research composes them. If `nitro` itself is not found, the CLI is not installed — stop and tell the user to install it: https://chillicream.com/docs/nitro/cli/installation. Do not attempt to install it yourself.

## Core principles

- **The lead never reads sources.** You (the main session) write the brief, dispatch, judge the synthesized result, and own every tracker write. Raw pages, files, and search results stay inside subagent contexts. Every raw token that reaches the lead is money spent twice.
- **Files are the channel, not chat.** Subagents write their output to the research folder and return a summary of at most ten lines. The next stage reads the files, not the previous agent's reply. The lead pastes that summary into the step's task comment, which is how the tracker becomes the log.
- **Cheapest model that can do the role.** Locating and extracting are cheap; judging and verifying are not. Give each role the lowest tier that does it reliably and escalate a single role only when its output fails the check below, never the whole pipeline.
- **Memory first, sources second.** A question the workspace already answered costs one search. Check before dispatching anything; save the answer when done.
- **Primary sources only.** Official docs, source code, specs, first-party APIs, RFCs. A blog post or a forum answer is a lead to a primary source, never a citation. Every claim in the findings names the source that owns it.
- **Facts, not decisions.** Research reports what is true and what is unknown. If the result forces a choice, the choice belongs to the caller (a grilling ticket, the user), not to this skill.

## The roles

- **Lead (you)**: sizes the job, writes the brief, creates and closes the step tasks, dispatches, checks outputs, writes the final answer paragraph, saves it to memory. Never fetches, never reads a source.
- **Scout**: turns the brief into a list of candidate primary sources (URLs, repo paths, doc sections) with one line on why each matters. Skims only; extracts nothing.
- **Reader**: one source per reader. Extracts the facts that bear on the sub-questions, each with a verbatim quote and a precise location (URL fragment, file path and line, section heading). Reports gaps in the source. Writes one note file.
- **Synthesizer**: reads all notes, writes the findings file: answer first, evidence with citations, conflicts between sources, unknowns. Never fetches new material; a gap becomes an unknown or a request back to the lead for one more reader.
- **Verifier**: for investigations only. Takes the findings, picks the load-bearing claims, re-opens the cited source for each and confirms or rejects it. Rejected claims are corrected or moved to unknowns.

Subagents never touch the tracker or memory; they do not have your actor name and should not learn the CLI. Roles are capabilities, not model names. For the model and effort per role and the spawning mechanics of your harness, read [references/claude-code.md](references/claude-code.md) or [references/codex.md](references/codex.md).

## Sizing

Pick the smallest shape that can answer the question. State the size in the brief and the epic.

| Size | When | Pipeline |
|---|---|---|
| lookup | one fact, source known or obvious | one reader, lead records the answer; no epic, one task |
| question | the default: a bounded question, sources to be found | scout, readers in parallel, synthesizer |
| investigation | a decision hangs on it, sources may conflict, or the cost of being wrong is high | question pipeline plus verifier |

If the question is really several questions, split it and run each as its own epic.

## The tracker structure

One epic per research job, labelled `research`, titled `Research: <question>`. Its description carries the question, the size, the folder path under `.nitro/research/`, and the caller (a ticket id, a user request). Each pipeline step is a child task (`--parent <epic>`), labelled `research:<role>`, with a `blocks` edge on the step it needs:

```
rs-7c1        Research: how does HotChocolate 16 handle @defer on connections   (epic)
rs-7c1.1      Scout sources                                                     research:scout
rs-7c1.2      Read: HC docs, Defer                                              research:read   depends on .1
rs-7c1.3      Read: incremental delivery RFC                                    research:read   depends on .1
rs-7c1.4      Read: HC source, DeferDirective.cs                                research:read   depends on .1
rs-7c1.5      Synthesize findings                                               research:synth  depends on .2 .3 .4
rs-7c1.6      Verify findings                                                   research:verify depends on .5   (investigation only)
```

Reader tasks are created after the scout closes, because that is when the sources are known. The lead claims a step before dispatching its subagent, comments the subagent's summary on it when it returns, and closes it with `--reason` naming the file written. A step whose subagent failed stays open with a comment saying what is missing; a rerun is a new dispatch on the same task, not a new task.

Why an epic: `nitro agent tasks ready` under it tells a resumed session exactly where the job stopped, `show` on the epic is the full log, and other sessions can see research in flight. Never link a child to the epic with bare `dep add`; `--parent` is the edge.

## Instructions

1. **Recall.** Search memory with the words the answer would contain: `nitro agent memory search "<words>" --output json`, then `nitro agent memory context --tag research --max-chars 4000`. A curated entry that answers the question ends the job: report it with its findings path and stop. A partial hit narrows the brief.
2. **Frame.** Restate the question in one sentence and list the sub-questions an answer must cover. Note what a complete answer looks like and what is explicitly out of scope. If the question hides a decision ("should we use X or Y?"), rewrite it as the facts the decision needs ("what does X guarantee about ordering; what does Y").
3. **Place the folder.** Research lives in the repo under `.nitro/research/<epic-id>-<slug>/`, so every session and every worktree finds it by convention. Create it once the epic id is known (step 4) and write `brief.md` there (template in [references/briefs.md](references/briefs.md)). If the caller wants research off the main branch, work in a separate worktree on `research/<slug>` and report the branch; nitro resolves the same workspace from every worktree.
4. **Open the epic.** Create it with `--type epic --label research`, take its id from `--output json` (never construct ids), create the folder named after it, and then the scout step (for a lookup, the single reader step instead). Put the folder path in the epic description.
5. **Scout** (question and investigation sizes). Claim the scout task, dispatch one scout with the brief path. It writes `sources.md`: candidate primary sources, ranked, with a reason each. Read `sources.md` yourself; it is short. Drop anything secondary. Cap at roughly eight sources; more than that means the question is too wide. Comment the summary, close the task, then create one reader task per kept source and the synthesizer task depending on all of them (and the verifier task depending on the synthesizer, for an investigation).
6. **Read.** Claim every reader task, dispatch one reader per source, all in parallel, each with the brief path, its single source, and its note path `notes/NN-<source-slug>.md`. Readers extract only what bears on the sub-questions. Check each returned summary for two failures: "no relevant content" (close the task with that reason, or retry with a sharper pointer) and claims without a location (re-run that reader one tier up on the same task). Comment and close each task as its reader returns.
7. **Synthesize.** Claim and dispatch the synthesizer with the folder path. It writes `findings.md` following the template. Read it. If a sub-question has no evidence, decide: one more reader task on a named source, or record it as unknown. Do not let the synthesizer fetch. Comment and close.
8. **Verify** (investigation size). Claim and dispatch the verifier with the folder path. It rewrites `findings.md` in place, marking each checked claim confirmed or corrected and moving unsupported ones to unknowns. Comment the verdict counts and close.
9. **Close.** Write the answer paragraph yourself in the reply and point at `findings.md`. Report the size run, the sources used, and the unknowns. Close the epic with `--reason` naming the findings path (`epic status` first; never `epic close-eligible`, it closes other workflows' epics too). Save the answer to memory: `nitro agent memory save "<answer paragraph, self-contained, with the findings path>" --type fact --tag research --tag <area> --actor <name>`; reuse an existing area tag from `memory tags`. Unknowns are not saved; they live in the findings and the epic. Do not touch the caller's tracker items; the caller records the resolution where it belongs.

**Resuming.** If you find an open `research` epic for the question (search the tracker before creating one), load it with `show`, run `ready` to find the next step, and continue from that step. Claimed steps held by another live session are theirs.

## Example

User: "Research how HotChocolate 16 handles `@defer` on connections; we may need it for the invoice list."

Lead recalls: `memory search "defer connections"` finds nothing. Sizes it **question**. Sub-questions: does the server support `@defer` on list and connection fields; what does the spec say about deferred fragments on non-nullable fields; what transport encoding is used; any documented limitations.

```bash
nitro agent tasks create "Research: how does HotChocolate 16 handle @defer on connections" \
  --actor maya --type epic --label research \
  --description "Size: question. Folder: .nitro/research/rs-7c1-hotchocolate-defer-connections. Caller: user request (invoice list)." --output json
nitro agent tasks create "Scout sources" --actor maya --parent rs-7c1 --label research:scout \
  --description "Find primary sources for the brief in .nitro/research/rs-7c1-hotchocolate-defer-connections/brief.md" --output json
nitro agent tasks update rs-7c1.1 --claim --actor maya
# dispatch scout; it returns: 4 sources, none for the @stream sub-question
nitro agent tasks comment add rs-7c1.1 "Scout: 4 primary sources, no source covers @defer with @stream. sources.md written." --actor maya
nitro agent tasks close rs-7c1.1 --actor maya --reason "sources.md written, 4 sources kept"
```

Folder at the end:

```
.nitro/research/rs-7c1-hotchocolate-defer-connections/
  brief.md
  sources.md            # scout: HC docs "Defer", spec incremental-delivery RFC, HC source DeferDirective.cs, HC changelog 16.x
  notes/
    01-hc-docs-defer.md
    02-graphql-incremental-delivery-rfc.md
    03-hc-source-defer-directive.md
    04-hc-changelog-16.md
  findings.md
```

Lead's reply, and the text saved to memory with `--type fact --tag research --tag graphql`:

> `@defer` on a connection field works in HotChocolate 16 and uses the incremental delivery multipart format; deferring a fragment that contains a non-null field makes the whole deferred payload nullable on error, per the RFC. Unknown: whether `@defer` composes with `@stream` on `edges`, the docs are silent and no test covers it. Details and citations in `.nitro/research/rs-7c1-hotchocolate-defer-connections/findings.md`.

Four readers ran on a cheap tier, the synthesizer one tier up, and the lead read three short files. No source page ever entered the lead's context. The next session that asks finds the answer in memory for the price of one search.

## Gotchas

- **A reader that summarizes instead of quoting is useless.** Location plus verbatim quote is the contract; a paraphrase cannot be verified later. Re-run, do not accept.
- **Secondary sources leak in through scouts.** Blog posts rank well in search. A scout may list one only as a pointer to the primary source it cites.
- **Do not pass the previous agent's reply into the next prompt.** Pass the folder path. Replies are summaries; files are the deliverable.
- **Do not escalate the whole pipeline.** When one reader fails on a dense source, rerun that reader one tier up. Everything else stays cheap.
- **Never close on a guess.** A source that could not be reached is an unknown in the findings, stated as such, and its reader task stays open with a comment. The caller decides whether to wait.
- **Memory holds the answer, not the evidence.** One self-contained paragraph with the findings path. The notes and the conflicts stay in the folder; a memory entry that needs the chat to make sense is lost.
- **A failed memory search is not proof.** Search is whole-word AND; retry with the words the answer would use before concluding the question is new.
- **Codebase facts are not research.** If the answer lives in the working directory, grep and read it in-session.

## References

- [references/briefs.md](references/briefs.md): file templates and the self-contained prompt for each role.
- [references/claude-code.md](references/claude-code.md): model and effort per role, spawning on Claude Code.
- [references/codex.md](references/codex.md): the same for the Codex CLI.
