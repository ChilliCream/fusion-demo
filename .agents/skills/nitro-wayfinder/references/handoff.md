# Handoff: from a cleared map to implementation tasks

When the map has no open tickets and **Not yet specified** is empty, the way is clear. The outcome of wayfinding is an implementation-ready task graph in the same tracker, written so that implementer agents can build from it without the chat that produced it, plus the effort's design documents in the repository. This session cuts the graph, verifies it, writes the design documents, closes the map, and briefs the orchestrator. If a destination turns out to need nothing built (a pure decision), the design documents are the deliverable: write them, close the map, and say so.

The orchestrator is a separate session running the sibling nitro-task-orchestrator skill. It takes the role `orchestrator`, groups ready tasks into waves by area label, and finds planners by role `planner`. Roles, not names, are how the two sides find each other: actor names are allocated per session.

## Before writing tasks

- Reread every closed decision ticket in full (`nitro agent tasks show <id> --output json`, including comments). The map's gists are not enough; the comments hold the locked parameters.
- Inspect the code that will change; for UI or behaviour work, look at the running app. Tasks written from memory produce implementer churn.
- Check for overlap: `nitro agent tasks search "<keyword>" --output json` and `nitro agent tasks list --status open --output json`. Update an existing task instead of duplicating it.
- A user ruling made while cutting tasks goes into the affected task as a comment, not only its description. Comments are the decision log implementers read.

## Structure

- One `epic` per code area (`storage`, `api`, `ui`, `jobs`...), titled `[<effort>] <area>`, labelled with the area. Implementation tasks are children of their area epic (`--parent`). The parent edge does not block the children; the orchestrator closes an epic once its children are done (`nitro agent tasks epic status`).
- Order with `blocks` edges from foundations to tests: storage before services, services before API, API before UI, everything before end-to-end tests. Cross-area interactions get an explicit note in the later task ("re-verify what <id> landed").
- Size each task for one implementer agent: one coherent change, verifiable on its own. Anything that needs two code areas becomes two linked tasks.
- Every locked parameter from the decision tickets goes into the task that implements it, by value, with a pointer to the ticket by name and id. Implementers treat the task as authoritative and will not go looking.

## Task quality bar

Every implementation task's description contains:

- **Problem**: what is missing or wrong, with evidence (file path, decision ticket).
- **File scope**: the concrete directories and files the implementer may touch. A boundary, not a hint.
- **Fix direction**: the intended approach, including the locked parameters. "Implement it" is not a plan.
- **Verification**: the commands or checks that prove it works. Name the real test filter or entry path.
- **Non-goals**: what is explicitly out of scope, especially adjacent cleanup an implementer would be tempted to do.

Plus the metadata the orchestrator schedules by: `--priority` 0-4, `--type` (`task`, `bug`, `feature`, `chore`), and the **area label**. An unlabelled task cannot be placed in a wave.

Example, with the jobs epic created as `bill-4a1`:

```bash
nitro agent tasks create "Write invoice CSV exporter" --actor maya \
  --parent bill-4a1 --type feature --priority 1 --label jobs --output json \
  --description "$(cat <<'EOF'
## Problem
Invoices cannot be exported. Decided in "Which export format?" (bill-3f2.1) and "Encoding and line endings" (bill-3f2.4).

## File scope
src/Billing.Jobs/Export/**, src/Billing.Jobs/Export.Tests/**

## Fix direction
CSV per RFC 4180, UTF-8 with BOM, CRLF, header row fixed to: id, issued_at, customer_id, total_cents, currency.
One file per day, named invoices-YYYY-MM-DD.csv, written to the finance share decided in "Where do exports land?" (bill-3f2.2): /finance/exports/invoices/.

## Verification
dotnet test src/Billing.Jobs/Export.Tests --filter Category=Export

## Non-goals
Scheduling ("Export schedule", bill-3f2.7), retention (out of scope on the map), any UI.
EOF
)"
```

## Verify the graph

```bash
nitro agent tasks dep cycles --output json                # {"items":[]}
nitro agent tasks lint --output json                      # no findings on the new tasks
nitro agent tasks ready --output json | jq '[.items[] | select(.id | startswith("bill-4a1."))]'   # per epic: only its foundations
```

## Design documents

The tracker holds the decisions; the repo holds what the team reads. Two documents go into the repository being worked on, in a folder per effort:

```text
.nitro/designs/<map-id>-<slug>/    # slug: kebab-case effort name, e.g. bill-3f2-billing-export
  decisions.md                     # the decision record: the audit log, one entry per closed ticket
  spec.md                          # the design spec: what we are building and why, written for colleagues
```

They have different readers. decisions.md is for whoever needs to know exactly what was settled and where the history is. spec.md is for a colleague who was not in the room: it must let them understand the change cold, and argue with it. Write decisions.md first, from the map and the closed tickets you reread before writing tasks. Then write spec.md from the map, the closed tickets, and any prototypes, with decisions.md open beside you. Ask the user nothing: the way is clear, so there is nothing left to decide, and a question here means the handoff started too early. Create the files in the working tree and leave them uncommitted; committing belongs to the user (git rules in operations.md).

Both documents are for sharing. Comments and edits on either are proposals, not decisions: a later revise session turns each into a ticket, resolves it, and rewrites the affected parts of both files. Say so in the header of each file so a reader knows how their input lands.

### decisions.md

One entry per closed decision ticket, in id order, carrying the resolution comment forward:

```markdown
# Billing export: decisions

Cut from map bill-3f2 at handoff. Each entry carries the named ticket's resolution; the ticket holds the full history.
Edits and comments here are proposals: a revise session turns them into tickets and updates spec.md.

## Destination
<the map's two lines>

## Which export format? (bill-3f2.1)
**Decision**: CSV per RFC 4180, UTF-8 with BOM, CRLF line endings.
**Rejected**: JSON lines (no consumer today); PDF bundle (finance needs cells, not pages).
**Locked parameters**: header row id, issued_at, customer_id, total_cents, currency; one file per day, invoices-YYYY-MM-DD.csv.

## Out of scope
<the map's Out of scope lines>
```

### spec.md

A design spec in the RFC sense: the problem, the proposed system as one piece, the forks taken, the risks, and the points where the team's input is wanted. It synthesizes; it does not restate decisions.md under other headings. A reader who finishes it should be able to say "I would have done X differently" and know which decision they are disagreeing with.

Rules:

- **Explain and connect, never decide.** Prose that walks the reader through the system, names trade-offs, and connects decisions is new text and is wanted. A sentence that settles something no ticket settled is a bug: stop, create the ticket, and resolve it before the spec claims it. Cite the ticket in parentheses wherever a statement rests on a decision, so a reviewer can pull the history; connective prose needs no citation.
- **Contracts in the form that is sharpest.** A schema, an API surface, a state machine, or a file format goes in as code, a table, or a diagram when prose would be vaguer. Trim to the decision-rich part. Keep file paths and implementation layout out: tasks carry those and they rot.
- **Alternatives at system level.** The two or three forks a colleague would plausibly re-open, what each would have bought, and why it lost. Per-ticket rejections stay in decisions.md.
- **Say where input is wanted.** The map's **For review** section collects, across the whole effort, the points the user flagged for the team; carry them into the spec verbatim, each pointing at the decision it concerns. A spec with nothing to review is a spec nobody needs to read.
- A pure-decision effort with no behaviour to specify writes decisions.md only, and says so in its header.

```markdown
# Billing export

Design spec for map bill-3f2, cut at handoff. Decisions are recorded in decisions.md; the ticket ids in parentheses lead to the history.
Comments and edits are proposals: a revise session turns them into tickets and updates both files.

## Problem
<who has the problem today, what it costs them, what happens if nothing is built>

## Proposal
<the shape of the change in one paragraph, then the main flow end to end as the user experiences it>

## How it works
<the system as one piece: components, the flow between them, the contracts as schema/API/state machine in code or tables, each contract citing its decision, e.g. "one file per day, invoices-YYYY-MM-DD.csv (bill-3f2.1)">

## Alternatives considered
<the system-level forks: what each would have bought, why it lost, which decision closed it>

## Trade-offs and risks
<what this design makes harder, what could fail in production, what it bets on>

## For review
<the points the team should weigh in on, each naming the decision it concerns and why the input matters>

## Rollout
<migration, compatibility, flags, order of shipping; omit the section if there is nothing>

## Out of scope
<what this effort deliberately does not do, and why it is past the destination>
```

Then close the map: `nitro agent tasks close bill-3f2 --actor maya --reason "Way clear; design at .nitro/designs/bill-3f2-billing-export/; implementation under [billing export] jobs (bill-4a1), [billing export] api (bill-4a2)"`.

## Brief the orchestrator

1. Find it: `nitro agent list --role orchestrator --output json`. If none is registered, report the created tasks to the user and stop; do not become the orchestrator. Mail to an unregistered name is accepted but reaches nobody.
2. Mail a compact briefing to the actor name that query returned, never an assumed one: `nitro agent mail send --to <orchestrator-actor> --actor maya --subject "[plan] billing export" --body "$(cat <<'EOF' ... EOF)"` with the epic and task ids (one line each, by name), area labels, the ordering constraints, any open question that needs a user ruling, and a note that tasks labelled `wayfinder:*` are decision tickets, never wave material. The orchestrator reads details with `show`; do not paste descriptions.
3. The briefing is stored before the orchestrator is woken, so a non-zero exit from `send` means the wake went unconfirmed, not that the mail was lost. If your harness can message another running session, send it a one-line pointer to the mail; otherwise the orchestrator drains its inbox between waves. If it mails back (ambiguous task, scope collision), fix the task and `nitro agent mail reply` on the same thread with what changed.

## What this session never does

Implement, claim or close implementation tasks, run waves, commit, push, switch branches, or touch the orchestrator's environment (dev server, browser, in-flight agents). The tracker is the interface; git state belongs to the user and the orchestrator.
