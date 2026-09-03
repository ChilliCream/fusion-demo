#!/usr/bin/env node
// Refreshes src/frontend/src/schema.graphql from the running gateway.
//
// The pinned relay-compiler (20.1.1) bundles graphql-js 15.3, whose parser
// rejects a `directive @tag(...) repeatable on ... | DIRECTIVE_DEFINITION`
// definition -- the composed gateway SDL declares that location, but
// graphql-js 15.3 does not yet know it. Upgrading relay-compiler is out of
// scope, so this script fetches the composed SDL and strips only the
// `DIRECTIVE_DEFINITION` location line from the `@tag` directive
// definition (nothing else) before writing the local schema file.
//
// Usage: yarn schema:refresh (run with the gateway up, e.g. via `aspire run`).

import { writeFile } from 'node:fs/promises'
import { fileURLToPath } from 'node:url'

const SDL_URLS = [
  'http://localhost:5116/graphql?sdl',
  'http://localhost:5116/graphql/schema.graphql',
]

const OUTPUT_PATH = fileURLToPath(
  new URL('../src/schema.graphql', import.meta.url),
)

async function fetchSdl() {
  const errors = []
  for (const url of SDL_URLS) {
    try {
      const response = await fetch(url)
      if (!response.ok) {
        errors.push(`${url} -> HTTP ${response.status}`)
        continue
      }
      const text = await response.text()
      if (text.trim().length === 0) {
        errors.push(`${url} -> empty response`)
        continue
      }
      return text
    } catch (error) {
      errors.push(`${url} -> ${error.message}`)
    }
  }
  throw new Error(
    `Could not fetch the composed SDL from the gateway. Is the stack running?\n${errors.join('\n')}`,
  )
}

// Removes only the `| DIRECTIVE_DEFINITION` location line from the `@tag`
// directive definition's location list, leaving every other directive
// definition (and every other location on @tag) untouched.
function stripTagDirectiveDefinitionLocation(sdl) {
  const lines = sdl.split('\n')
  const tagStart = lines.findIndex((line) =>
    line.startsWith('directive @tag('),
  )
  if (tagStart === -1) {
    throw new Error('Could not find a `directive @tag(...)` definition in the fetched SDL.')
  }

  let blockEnd = tagStart + 1
  while (blockEnd < lines.length && /^\s*\|/.test(lines[blockEnd])) {
    blockEnd++
  }

  const removeIndex = lines.findIndex(
    (line, index) =>
      index >= tagStart &&
      index < blockEnd &&
      line.trim() === '| DIRECTIVE_DEFINITION',
  )
  if (removeIndex === -1) {
    throw new Error(
      'Could not find `| DIRECTIVE_DEFINITION` under the `@tag` directive definition in the fetched SDL. ' +
        'The composed schema may have changed -- check whether the graphql-js parse limitation is still in play.',
    )
  }

  lines.splice(removeIndex, 1)
  return lines.join('\n')
}

async function main() {
  const sdl = await fetchSdl()
  const patched = stripTagDirectiveDefinitionLocation(sdl.trimEnd())
  await writeFile(OUTPUT_PATH, patched)
  console.log(`Wrote ${OUTPUT_PATH}`)
}

main().catch((error) => {
  console.error(error.message)
  process.exit(1)
})
