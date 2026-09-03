# Grilling: resolving a decision with the human

Grilling is how HITL tickets get resolved, and how the destination and the initial frontier get named while charting. Interview the user relentlessly, scoped to the one decision at hand, until you reach a shared understanding. Map the decision as a **design tree**: every answer branches into the decisions that hang off it.

## Settle the question style first

Before the first question of an effort, agree how the user wants to be asked. Check `nitro agent memory context --tag <memory tag>` first: if a question-style preference is already saved, follow it and never re-ask. Otherwise put it to them with the AskUserQuestion tool, which renders as a two-option picker:

- **Batched rounds** -- the whole frontier in one round, numbered, each with a recommendation. Fewer turns; the user scans and answers several at once.
- **One at a time** -- one question, never the selector UI. Issue in three simple sentences, an example if it helps, recommendation in three sentences. Then discuss it to agreement before the next one.

Save the answer as soon as the map exists, so every later session inherits it:

```bash
nitro agent memory save "Wayfinder question style: one question at a time, never the selector UI. Issue in three simple sentences, an example if it helps, recommendation in three sentences; discuss to agreement before the next question." \
  --type preference --tag wayfinder-billing-export --actor maya
```

The style governs delivery only. Everything else here -- the design tree, the frontier, self-contained questions, facts are your job, decisions are the user's -- holds in both. Never switch style mid-effort unless the user asks; if they do, save the new preference over the old with `memory update`.

## Style A: batched rounds over the frontier

Work the tree in rounds. The frontier of a round is every question whose prerequisites are already settled: the questions you can ask now without guessing at answers you have not heard. Ask the whole frontier in one round, then wait. A question whose answer depends on another question still open in this round belongs to a later round.

Deliver the round with the AskUserQuestion tool: up to four questions per call, each with two to four options, and an "Other" free-text escape added for you. Put your recommendation first and mark it `(Recommended)`. The question text still has to stand alone -- the picker shows it with no surrounding chat.

```
header:   "Export format"
question: "Invoices are read by finance's Excel import and by a nightly reconciliation job.
           One format has to serve both. Which do we export?"
options:  CSV (RFC 4180) (Recommended) -- both consumers already parse it; Excel needs no plugin
          JSON lines                   -- richer, but no current consumer reads it
          Both                          -- two code paths to keep in sync forever
```

A frontier wider than four questions goes out as consecutive calls, not a trimmed round. A question whose answers are not a short discrete set does not belong in a picker: ask that one in prose, using the Style B shape below, in the same round.

Each answer reshapes the tree: settled decisions push the frontier outward and unblock what depended on them. Recompute the frontier and ask the next round.

## Style B: one question at a time

One question at a time, never the selector UI. Three labelled parts, in simple English: short sentences, common words, no jargon the user did not introduce.

```
**Issue**: Invoices leave the system as a file, and two very different consumers read it. Finance opens it in Excel on Windows; a nightly reconciliation job parses it unattended. One format has to serve both, and the wrong pick means a second export path later.

**Example**:

    InvoiceId,IssuedOn,NetAmount,Currency
    INV-1001,2026-08-01,1240.00,EUR

**Recommendation**: CSV per RFC 4180 with a fixed header row. Both consumers already parse CSV, and Excel imports it without a plugin. JSON lines would add a second code path no current consumer needs.
```

- **Issue**: three simple sentences explaining what the question is about -- what the problem really is, not a list of the candidate answers.
- **Example**: optional; include it if it helps. A bit of C#, GraphQL SDL, or a payload, just so the problem is visualized. Skip it when prose is clearer.
- **Recommendation**: three sentences on what you recommend. Commit to one answer.

Then discuss this point until you and the user agree on a solution, and only then present the next question. Expect pushback, counter-examples, and a changed mind; the style buys a real conversation per decision, so do not rush it. When you agree, state the agreement in one line and move on. Never batch two questions because they feel related -- the whole point is one thing on the table at a time.

Research is unaffected: a fact you need is still a subagent's job, never a question, and it runs alongside the conversation instead of stalling it.

## Rules

- **Every question is self-contained.** The context sits inside the question. Never "as discussed above" or a bare option list; a question that gets "what?" back lacked context, so rewrite it in plain prose and ask again.
- **Terse, plain language.** Short sentences. No jargon the user did not introduce.
- **Facts are your job, never the user's.** When a question needs a fact from the environment (what the schema looks like, what a library supports, what a vendor's API does), dispatch a subagent to find it. Do not block the round on it: only the questions downstream of that fact wait; ask the rest now. Feed the finding into the next round. A fact only this ticket needs goes into this ticket's resolution comment; a fact other tickets will depend on becomes a research ticket with a `blocks` edge (see research.md).
- **Decisions are the user's.** Put each to them and wait. Never fill in the user's side of the exchange, and never treat a recommendation as an answer.
- **If the user declines to answer**, re-ask the same round or question once when they say "ask again". If they answer with confusion instead of a choice, the question lacked context, not the user: rewrite it. In Style A, a dismissed picker is a decline, not an answer.
- **Log progress to the ticket, not to chat.** After every agreed point (Style B) or every answered round (Style A), add a progress comment to the ticket before asking the next question: what was agreed, and which branches are still open (template in operations.md). A ruling given mid-round goes there too. Chat holds only the question on the table; the ticket holds the state of the grill.

## Logging progress and resuming

The design tree is not in your head. Its state is the ticket's latest progress comment: an **Agreed** list that only grows, and an **Open** list of the branches still to visit. Write the comment before the next question goes out, so the board is never more than one question behind the conversation. When a fact arrives from a subagent, log it as an agreed point the same way.

Before the first question of a claimed ticket, read its comments (`show <id> --output json`). A progress comment from an earlier session means the grill is half done: restate the last **Agreed** list to the user in one short paragraph, then continue from **Open**. Never re-ask an agreed point; if the user wants to reopen one, log the change as a new agreed point that supersedes the old.

## Model the domain as you grill

Decisions are made in words, so sharpen them while you ask. Challenge terms that are vague or overloaded ("export" the file, or "export" the job?) and propose one canonical term. When the user states how something works, invent an edge case and test the statement against it. If the repository keeps a glossary or decision records (a `CONTEXT.md`, `docs/adr/`), read them before the first round and put resolved terms and decisions there in the same session, following the repository's convention; the ticket comment links to them.

## When the decision lands

The latest progress comment's **Open** list is empty: every branch visited, nothing silently assumed. Restate the decision in one short paragraph and ask the user to confirm the shared understanding. On confirmation, immediately write the resolution comment (decision, rejected options and why, every locked parameter; template in operations.md), compiled from the progress comments, close the ticket, append to the map, and run the graduation pass. Do not act on the decision before the confirmation, and do not let further chat delay the write after it.

While grilling, listen for the user wanting the team's input: "let's see what X thinks", "I'm not sure the finance lead agrees", "we should ask the team". Each such point is a line under the map's **For review** section, naming the ticket and why the input matters. The spec written at handoff carries them to the team; a point never written down is a review the team never gets asked for.

## Charting tickets

Charting runs as two grilling tickets under the map, created before the first question (see operations.md): **Name the destination** and **Map the frontier**. They are ordinary tickets: claimed, logged, resolved, and picked up by "next" if the charting session dies.

**Name the destination** pins the destination in two lines; it fixes scope. Its resolution comment's Decision is those two lines, which then replace the map's `Draft:` destination.

**Map the frontier** is breadth-first: fan out across the whole space rather than deep on any one thread, to surface the decisions that can be stated now and the fog that cannot. Depth belongs to the ticket sessions, not to charting. Each decision the user agrees is statable becomes a ticket at once, and each fog patch goes into the map's **Not yet specified** at once; the progress comment names the ticket ids created so far. Its resolution comment's Decision lists the tickets and fog it produced.
