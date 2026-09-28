# Prototype tickets: answering a question by building

A prototype is throwaway work that answers one question. Use it when conversation cannot: "does this design hold up on the real database version", "does this state model feel right", "what should this screen look like". The question decides the shape of the artifact.

Prototype tickets are HITL: the artifact exists for the human to react to, and the reaction is the decision. The build itself can run as a subagent.

## Shapes

- **Does it work?** A minimal runnable spike against the real dependency: a container of the actual database version, the real DDL, scenario queries; or a script hitting the real API. The output is evidence, not code to keep. State the pass and fail criteria before building.
- **Does the logic feel right?** A tiny interactive program that pushes the state model through the cases that are hard to reason about on paper. Keep the model a pure module with the domain's own words as labels, drive it with scenario inputs, and print the full state after every step so the human sees what changed.
- **What should it look like?** Several structurally different variants of one screen on the existing route, switchable in place (a query parameter and a small switcher hidden outside the prototype), read-only, with realistic domain data. The human compares rather than imagines.

If the question is ambiguous and the human is not reachable, pick the shape that matches the surrounding code (a backend module: logic; a page or component: UI) and state the assumption at the top of the artifact.

## Rules

1. **Throwaway from the first line, and marked as such.** Put it near the code it informs, name it so nobody mistakes it for production, and keep it off the main branch: the subagent commits it in a separate worktree on a `prototype/<slug>` branch (see Git rules in operations.md) and the ticket links the branch.
2. **One command to run.** The human must be able to start it without thinking.
3. **No persistence, no polish.** No tests, no error handling beyond what makes it run, no abstractions. Scratch databases get a name that says "wipe me".
4. **Bound it.** Set maximum execution times on queries and timeouts on calls. A subagent stuck on a hanging query yields nothing.
5. **Prototypes may fail the design.** A falsified design is a successful prototype. Record the failure and what it implies; that is usually where the next tickets come from.
6. **Capture the verdict where future sessions look.** The subagent writes `findings.md` next to the artifact; if it cannot write files, it returns the findings and the dispatching session commits them in the worktree. The resolution comment on the ticket carries the verdict and the locked parameters; the branch carries the evidence.

## Resolving

Show the human the artifact or its findings, grill the reaction in rounds (see grilling.md), confirm the shared understanding, then write the resolution comment (template in operations.md: decision, rejected, locked parameters, assets = the branch) and close:

```bash
nitro agent tasks close bill-3f2.4 --actor maya --reason "Prototyped: naive nightly rebuild too slow at 10M rows; incremental design instead"
```

Append the gist to the map and run the graduation pass. A falsified design typically closes some tickets, opens others, and sharpens fog.
