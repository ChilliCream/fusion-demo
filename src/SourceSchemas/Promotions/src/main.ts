import { createServer } from "node:http";
import { writeFileSync } from "node:fs";
import { fileURLToPath } from "node:url";
import { createYoga } from "graphql-yoga";
import {
  createSourceSchemaHandler,
  printSourceSchema
} from "graphql-federation-subgraph";
import { schema, type PromotionsContext } from "./schema.js";
import { createPromotionStore } from "./store.js";

// Mirrors ExportSchemaOnStartup of the .NET source schemas: keep the checked-in
// schema.graphqls in sync with the executable schema outside of production.
if (process.env.NODE_ENV !== "production") {
  writeFileSync(
    fileURLToPath(new URL("../schema.graphqls", import.meta.url)),
    printSourceSchema(schema)
  );
}

// Applies pending migrations when a database is configured, otherwise falls
// back to the in-memory promotions.
const store = await createPromotionStore();

const context: PromotionsContext = { store };
const yoga = createYoga({ schema, context, graphqlEndpoint: "/graphql" });
const schemaHandler = createSourceSchemaHandler(schema);
const port = Number(process.env.PORT ?? 5118);

createServer((req, res) => {
  if (req.url?.split("?")[0] === "/graphql/schema.graphql") {
    schemaHandler(req, res);

    return;
  }

  yoga(req, res);
}).listen(port, () => {
  console.log(`Promotions API listening on http://localhost:${port}/graphql`);
});
