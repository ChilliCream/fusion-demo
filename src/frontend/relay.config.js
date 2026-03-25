const { existsSync, writeFileSync } = require("fs");
const { resolve } = require("path");

const enablePersistedOperations =
  String(process.env.VITE_ENABLE_PERSISTED_OPERATIONS).toLowerCase() === "true";

const config = {
  src: "./src",
  language: "typescript",
  schema: "./src/schema.graphql",
  excludes: ["**/node_modules/**", "**/__mocks__/**", "**/__generated__/**"],
  eagerEsModules: true,
};

if (enablePersistedOperations) {
  const operationsFilePath = resolve(__dirname, "operations.json");

  if (!existsSync(operationsFilePath)) {
    writeFileSync(operationsFilePath, "{}\n", "utf8");
  }

  config.persistConfig = {
    file: "./operations.json",
    algorithm: "MD5",
  };
}

module.exports = config;
