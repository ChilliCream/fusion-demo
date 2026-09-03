# Briefs: file templates and role prompts

Every file lives under the research folder `.nitro/research/<epic-id>-<slug>/`. Every prompt below is pasted whole into the subagent's prompt, with the placeholders filled. Subagents write files and return a summary of at most ten lines.

## brief.md (written by the lead)

```markdown
# Research: <one-sentence question>

Size: lookup | question | investigation

## Sub-questions
1. <fact the answer must establish>
2. <...>

## Complete when
<what a finished answer contains>

## Out of scope
- <thing that looks related and is not>

## Known starting points
- <URL, repo path, or "none">
```

## sources.md (written by the scout)

```markdown
# Sources

| # | Source | Type | Why it matters | Covers |
|---|---|---|---|---|
| 1 | https://... | official docs | ... | Q1, Q3 |
| 2 | src/.../Foo.cs | source code | ... | Q2 |

## Rejected
- <secondary source>: points at #1, not cited itself
```

## notes/NN-<source-slug>.md (written by a reader)

```markdown
# <source title>
Source: <URL or path>
Retrieved: <date>

## Facts
### Q1
- Claim: <one sentence in your words>
  Quote: "<verbatim, trimmed to the sentence that carries the claim>"
  Location: <URL#fragment | path:line | section heading>
- ...

## Not covered here
- Q3: the source does not address it

## Doubts
- <anything ambiguous, versioned, or deprecated in the source>
```

## findings.md (written by the synthesizer, rewritten by the verifier)

```markdown
# <question>

## Answer
<one paragraph; the reader should be able to stop here>

## Evidence
### Q1
<claim> [notes/01, Location]
<claim> [notes/03, Location]

### Q2
...

## Conflicts
- <source A says X, source B says Y; which is authoritative and why>

## Unknown
- <sub-question or detail no primary source settled, and what was tried>

## Sources
1. <URL or path> (notes/01)
```

The verifier appends `Verified: confirmed | corrected` after each claim it checked, and a `## Verification` section listing what it re-opened and what it rejected.

## Role prompts

### Scout

```
You are the scout in a research pipeline. Read <folder>/brief.md.
Find primary sources that can answer its sub-questions: official documentation, source code, specifications, RFCs, first-party API references. Search, then open each candidate only far enough to confirm it is primary and relevant.
Write <folder>/sources.md using the sources.md template: ranked, at most eight, each with the sub-questions it covers. List secondary sources you found under Rejected with the primary source they point to; never rank them.
Do not extract facts. Return at most ten lines: how many sources, and any sub-question with no source found.
```

### Reader

```
You are a reader in a research pipeline. Read <folder>/brief.md. Your single source is: <URL or path>.
Open the source. For each sub-question it addresses, record every relevant fact as a claim with a verbatim quote and an exact location (URL fragment, file path and line, or section heading). Do not paraphrase in place of quoting. Do not consult any other source. Note what the source does not cover and anything ambiguous or version-specific.
Write <folder>/notes/<NN-slug>.md using the notes template.
Return at most ten lines: which sub-questions got evidence, which got none, and whether the source was reachable.
```

### Synthesizer

```
You are the synthesizer in a research pipeline. Read <folder>/brief.md, then every file in <folder>/notes/.
Write <folder>/findings.md using the findings template. Lead with the answer. Every claim in Evidence cites a note and location. Where notes disagree, write it under Conflicts and say which source owns the truth. A sub-question with no evidence goes under Unknown with what was tried; do not fill gaps from your own knowledge and do not fetch anything.
Return at most ten lines: the answer in one sentence, and each unknown.
```

### Verifier

```
You are the verifier in a research pipeline. Read <folder>/findings.md and the notes it cites.
Pick the claims the Answer depends on. For each, re-open the cited source at the cited location and confirm the quote and the claim. Mark each checked claim "Verified: confirmed" or rewrite it and mark "Verified: corrected". Move any claim you cannot support to Unknown. Add a "## Verification" section listing what you checked and what you rejected. Edit <folder>/findings.md in place.
Return at most ten lines: claims checked, confirmed, corrected, rejected.
```
