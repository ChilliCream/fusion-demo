# Product API - Node.js GraphQL Server

A Node.js GraphQL server with variable batching support for product data.

## Features

- GraphQL API with the exact schema from `schema.graphqls`
- Variable batching support for efficient batch requests
- Product data matching the C# ProductContext
- CORS enabled for cross-origin requests
- Health check endpoint
- GraphQL Playground for testing

## Installation

```bash
npm install
```

## Running the Server

### Development

```bash
npm run dev
```

### Production

```bash
npm start
```

The server will start on port 4000 by default.

## API Endpoints

- **GraphQL**: `POST http://localhost:4000/graphql`
- **GraphQL Playground**: `GET http://localhost:4000/graphql-playground`
- **Health Check**: `GET http://localhost:4000/health`

## Variable Batching

The server supports two execution modes based on the `variables` field:

### Standard GraphQL Execution

When `variables` is an object, null, or undefined:

```json
{
  "query": "query ($id: ID!) { productById(id: $id) { name price } }",
  "variables": {
    "id": "1"
  }
}
```

### Batch Execution

When `variables` is an array, the server executes the same query multiple times with different variable sets:

```json
{
  "query": "query ($id: ID!) { productById(id: $id) { name price } }",
  "variables": [
    { "id": "1" },
    { "id": "2" },
    { "id": "3" }
  ]
}
```

This returns an array of results, one for each variable set.

## Available Products

The server includes the following products:

1. **Table** (ID: "1") - $899.99, 100kg
2. **Couch** (ID: "2") - $1299.50, 1000kg  
3. **Chair** (ID: "3") - $54, 50kg

## Example Queries

### Get a single product

```graphql
query {
  productById(id: "1") {
    id
    name
    price
    weight
    dimension {
      length
      width
      height
    }
  }
}
```

### Get all products

```graphql
query {
  products {
    nodes {
      id
      name
      price
      weight
    }
  }
}
```

### Batch query example

```bash
curl -X POST http://localhost:4000/graphql \
  -H "Content-Type: application/json" \
  -d '{
    "query": "query ($id: ID!) { productById(id: $id) { name price } }",
    "variables": [
      { "id": "1" },
      { "id": "2" },
      { "id": "3" }
    ]
  }'
```

## Schema

The server implements the complete GraphQL schema from `schema.graphqls`, including:

- Product types with dimensions
- Pagination support (ProductsConnection)
- Node interface
- Error handling
- Custom scalars (URL, Upload)
