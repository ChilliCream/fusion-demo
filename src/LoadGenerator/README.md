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
- Supports weighted selection to control how often each query is chosen

### McpWorker

Calls tools on the gateway's MCP server at `/graphql/mcp` using the `McpClient` from `ModelContextProtocol.Core`.

- Reads tool definitions from `configuration/tools.json`
- Randomly selects argument values from sample arrays defined in the configuration
- Supports weighted selection to control how often each tool is called

### OpenApiWorker

Sends HTTP GET requests to the gateway's REST/OpenAPI endpoints.

- Reads endpoint definitions from `configuration/endpoints.json`
- Replaces path placeholders (e.g. `{id}`) with randomly selected sample values
- Remaining parameters are added as query string parameters
- Parameters with a randomly selected `null` value are omitted from the request
- Supports weighted selection to control how often each endpoint is hit

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

The `configuration/` directory contains the data files that define what requests each worker sends. Each file is a JSON array of objects with optional sample values for randomization.

All configuration entries support a `weight` property (integer, defaults to `1`) that controls how often the entry is selected relative to others. For example, an entry with `weight: 10` is 10x more likely to be picked than one with `weight: 1`. The same operation ID, route, or tool name can appear multiple times with different parameters and weights.

### `configuration/queries.json` (GraphQL)

An array of query objects. Each entry has:

- `id` (required) — the persisted operation ID (hash)
- `query` (required) — the full GraphQL query string
- `variables` (optional) — an object where each key is a variable name and the value is an array of sample values
- `weight` (optional) — selection weight, defaults to `1`

```json
[
  {
    "id": "f1a216fa82107e781221cbecd539efd2",
    "query": "query ProductsPageQuery($count: Int!, $cursor: String) { ... }",
    "variables": {
      "count": [1, 5, 10, 15],
      "cursor": [null, "abc123", "def456"]
    },
    "weight": 10
  },
  {
    "id": "f1a216fa82107e781221cbecd539efd2",
    "query": "query ProductsPageQuery($count: Int!, $cursor: String) { ... }",
    "variables": {
      "count": [1]
    },
    "weight": 1
  }
]
```

### `configuration/tools.json` (MCP)

An array of tool objects. Each entry has:

- `name` (required) — the tool name
- `arguments` (optional) — an object where each key is an argument name and the value is an array of sample values
- `weight` (optional) — selection weight, defaults to `1`

```json
[
  {
    "name": "SearchProducts",
    "arguments": {
      "text": ["Chair", "Table", "Bookshelf"],
      "first": [1, 5, 10, 15, 20]
    },
    "weight": 5
  }
]
```

### `configuration/endpoints.json` (OpenAPI)

An array of endpoint objects. Routes can contain `{placeholder}` segments. Each entry has:

- `route` (required) — the route path
- `parameters` (optional) — an object where each key is a parameter name and the value is an array of sample values
- `weight` (optional) — selection weight, defaults to `1`

Parameters matching a path placeholder replace it in the URL. All other parameters are appended as query string parameters. A randomly selected `null` value causes the parameter to be omitted.

```json
[
  {
    "route": "/products/{id}",
    "parameters": {
      "id": ["UHJvZHVjdDox", "UHJvZHVjdDoy"]
    },
    "weight": 5
  },
  {
    "route": "/products/search",
    "parameters": {
      "text": ["Chair", "Table"],
      "minPrice": [null, 100, 500],
      "first": [5, 10, 20]
    },
    "weight": 1
  }
]
```
