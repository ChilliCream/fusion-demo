const { makeExecutableSchema } = require('graphql-tools');

// Build the GraphQL schema from the provided schema.graphqls
const typeDefs = `
  scalar URL

  interface Error {
    message: String!
  }

  interface Node {
    id: ID!
  }

  type PageInfo {
    hasNextPage: Boolean!
    hasPreviousPage: Boolean!
    startCursor: String
    endCursor: String
  }

  type Product implements Node {
    dimension: ProductDimension!
    pictureUrl: URL
    id: ID!
    name: String!
    price: Float!
    weight: Int!
    pictureFileName: String
  }

  type ProductDimension {
    length: Float!
    width: Float!
    height: Float!
  }

  type ProductsConnection {
    pageInfo: PageInfo!
    edges: [ProductsEdge!]
    nodes: [Product!]
  }

  type ProductsEdge {
    cursor: String!
    node: Product!
  }

  type Query {
    node(id: ID!): Node
    nodes(ids: [ID!]!): [Node]!
    productById(id: ID!): Product
    products(first: Int, after: String, last: Int, before: String): ProductsConnection
  }

  type UnknownProductError implements Error {
    productId: Int!
    message: String!
  }

  type UploadProductPicturePayload {
    product: Product
    errors: [UploadProductPictureError!]
  }

  union UploadProductPictureError = UnknownProductError

  input UploadProductPictureInput {
    productId: Int!
    picture: Upload!
  }

  scalar Upload
`;

module.exports = typeDefs;
