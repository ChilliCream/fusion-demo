import { writeFileSync } from "node:fs";
import { fileURLToPath } from "node:url";
import { printSourceSchema } from "graphql-federation-subgraph";
import { schema } from "./schema.js";

const target = fileURLToPath(new URL("../schema.graphqls", import.meta.url));

writeFileSync(target, printSourceSchema(schema));

console.log(`Exported source schema to ${target}`);
