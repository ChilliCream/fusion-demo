# Load Generator

A .NET Worker Service that generates load against the Fusion gateway by continuously sending randomized requests across three protocols: GraphQL, MCP (Model Context Protocol), and OpenAPI (REST).

## Running

```bash
dotnet run --project src/LoadGenerator
```

## Workers

The load generator runs three independent background workers simultaneously. Each worker can be enabled/disabled and configured independently.

### GraphQLWorker

Sends GraphQL queries to the gateway's `/graphql` endpoint using Hot Chocolate's `DefaultGraphQLHttpClient`.

- Reads queries from `configuration/queries.json`
- Supports persisted operations (sends only the operation ID) or full query mode (sends the complete query body)
- Randomly selects variable values from sample arrays defined in the configuration

### McpWorker

Calls tools on the gateway's MCP server at `/graphql/mcp` using the `McpClient` from `ModelContextProtocol.Core`.

- Reads tool definitions from `configuration/tools.json`
- Randomly selects argument values from sample arrays defined in the configuration

### OpenApiWorker

Sends HTTP GET requests to the gateway's REST/OpenAPI endpoints.

- Reads endpoint definitions from `configuration/endpoints.json`
- Replaces path placeholders (e.g. `{id}`) with randomly selected sample values
- Remaining parameters are added as query string parameters
- Parameters with a randomly selected `null` value are omitted from the request

## Configuration

All settings are in `appsettings.json` under the `LoadGenerator` section. Environment-specific overrides go in `appsettings.Development.json`.

### Options

| Option | Scope | Default | Description |
|--------|-------|---------|-------------|
| `GatewayUrl` | Global | `http://localhost:5000` | Base URL of the Fusion gateway (without `/graphql`) |
| `MinDelayMs` | Per worker | `500` | Minimum delay (ms) between batches |
| `MaxDelayMs` | Per worker | `3000` | Maximum delay (ms) between batches |
| `MaxConcurrentRequests` | Per worker | `5` | Maximum number of concurrent requests per batch |
| `MaxRequestsPerBatch` | Per worker | `10` | Upper bound for the number of requests in a batch (actual count is randomized from 1 to this value) |
| `UsePersistedOperations` | GraphQL only | `true` | When `true`, sends only the operation ID; when `false`, sends the full query body |
| `Enabled` | Per worker | `true` | Whether the worker is active |

### Example

```json
{
  "LoadGenerator": {
    "GatewayUrl": "https://demo.chillicream-graphql.com",
    "GraphQL": {
      "MinDelayMs": 500,
      "MaxDelayMs": 3000,
      "MaxConcurrentRequests": 5,
      "MaxRequestsPerBatch": 10,
      "UsePersistedOperations": true,
      "Enabled": true
    },
    "Mcp": {
      "MinDelayMs": 500,
      "MaxDelayMs": 3000,
      "MaxConcurrentRequests": 5,
      "MaxRequestsPerBatch": 10,
      "Enabled": true
    },
    "OpenApi": {
      "MinDelayMs": 500,
      "MaxDelayMs": 3000,
      "MaxConcurrentRequests": 5,
      "MaxRequestsPerBatch": 10,
      "Enabled": true
    }
  }
}
```

To disable a specific worker, set its `Enabled` to `false`:

```json
{
  "LoadGenerator": {
    "Mcp": {
      "Enabled": false
    }
  }
}
```

## Configuration Data

The `configuration/` directory contains the data files that define what requests each worker sends. Each file maps identifiers to request definitions with optional sample values for randomization.

### `configuration/queries.json` (GraphQL)

Keyed by persisted operation ID (hash). Each entry has:

- `query` (required) — the full GraphQL query string
- `variables` (optional) — an object where each key is a variable name and the value is an array of sample values

```json
{
  "f1a216fa82107e781221cbecd539efd2": {
    "query": "query ProductsPageQuery($count: Int!, $cursor: String) { ... }",
    "variables": {
      "count": [1, 5, 10, 15],
      "cursor": [null, "abc123", "def456"]
    }
  }
}
```

### `configuration/tools.json` (MCP)

Keyed by tool name. Each entry has:

- `arguments` (optional) — an object where each key is an argument name and the value is an array of sample values

```json
{
  "SearchProducts": {
    "arguments": {
      "text": ["Chair", "Table", "Bookshelf"],
      "first": [1, 5, 10, 15, 20]
    }
  }
}
```

### `configuration/endpoints.json` (OpenAPI)

Keyed by route path. Routes can contain `{placeholder}` segments. Each entry has:

- `parameters` (optional) — an object where each key is a parameter name and the value is an array of sample values

Parameters matching a path placeholder replace it in the URL. All other parameters are appended as query string parameters. A randomly selected `null` value causes the parameter to be omitted.

```json
{
  "/products/{id}": {
    "parameters": {
      "id": ["UHJvZHVjdDox", "UHJvZHVjdDoy"]
    }
  },
  "/products/search": {
    "parameters": {
      "text": ["Chair", "Table"],
      "minPrice": [null, 100, 500],
      "first": [5, 10, 20]
    }
  }
}
```
