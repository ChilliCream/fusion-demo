import { GraphQLError } from "graphql";
import { buildSubgraphSchema } from "graphql-federation-subgraph";
import { promotionForProduct, type CreatePromotionInput, type Promotion } from "./data.js";
import type { PromotionStore } from "./store.js";

export interface PromotionsContext {
  store: PromotionStore;
}

const typeDefs = /* GraphQL */ `
  schema {
    query: Query
    mutation: Mutation
  }

  type Query {
    "Fetches the currently running promotions."
    promotions: [Promotion!]! @cost(weight: "10")
    promotionById(id: ID!): Promotion @lookup @cost(weight: "10")
    productById(id: ID!): Product @lookup @internal
  }

  type Mutation {
    "Creates a new promotion that products across the catalog can take part in."
    createPromotion(input: CreatePromotionInput!): CreatePromotionPayload! @cost(weight: "10")
  }

  input CreatePromotionInput {
    title: String!
    description: String
    "The discount in percent that the promotion takes off the list price."
    discountPercent: Int!
  }

  type CreatePromotionPayload {
    promotion: Promotion
  }

  "A discount campaign that products across the catalog take part in."
  type Promotion @key(fields: "id") {
    id: ID!
    title: String!
    description: String
    "The discount in percent that the promotion takes off the list price."
    discountPercent: Int!
  }

  type Product @key(fields: "id") {
    "The promotion this product currently takes part in, if any."
    promotion: Promotion @cost(weight: "10")
    "The list price after applying the discount of the current promotion."
    discountedPrice(price: Float! @require(field: "price")): Float! @cost(weight: "10")
    id: ID!
  }

  "The purpose of the \`cost\` directive is to define a \`weight\` for GraphQL types, fields, and arguments. Static analysis can use these weights when calculating the overall cost of a query or response."
  directive @cost(
    "The \`weight\` argument defines what value to add to the overall cost for every appearance, or possible appearance, of a type, field, argument, etc."
    weight: String!
  ) on
    | SCALAR
    | OBJECT
    | FIELD_DEFINITION
    | ARGUMENT_DEFINITION
    | ENUM
    | INPUT_FIELD_DEFINITION
`;

interface ProductRef {
  id: string;
}

const resolvers = {
  Query: {
    promotions: (
      _parent: unknown,
      _args: unknown,
      context: PromotionsContext
    ): Promise<Promotion[]> => context.store.listPromotions(),
    promotionById: (
      _parent: unknown,
      args: { id: string },
      context: PromotionsContext
    ): Promise<Promotion | null> => context.store.getPromotionById(args.id),
    productById: (_parent: unknown, args: { id: string }): ProductRef => ({
      id: args.id
    })
  },
  Mutation: {
    createPromotion: async (
      _parent: unknown,
      args: { input: CreatePromotionInput },
      context: PromotionsContext
    ): Promise<{ promotion: Promotion }> => {
      const { title, discountPercent } = args.input;

      if (title.trim().length === 0) {
        throw new GraphQLError("The title cannot be empty.", {
          extensions: { code: "PROMOTION_TITLE_EMPTY" }
        });
      }

      if (discountPercent < 1 || discountPercent > 100) {
        throw new GraphQLError(
          "The discount percent must be between 1 and 100.",
          { extensions: { code: "PROMOTION_DISCOUNT_OUT_OF_RANGE" } }
        );
      }

      return { promotion: await context.store.createPromotion(args.input) };
    }
  },
  Product: {
    promotion: async (
      parent: ProductRef,
      _args: unknown,
      context: PromotionsContext
    ): Promise<Promotion | null> =>
      promotionForProduct(parent.id, await context.store.listPromotions()),
    discountedPrice: async (
      parent: ProductRef,
      args: { price: number },
      context: PromotionsContext
    ): Promise<number> => {
      const promotion = promotionForProduct(
        parent.id,
        await context.store.listPromotions()
      );

      if (promotion === null) {
        return args.price;
      }

      return (
        Math.round(
          ((args.price * (100 - promotion.discountPercent)) / 100) * 100
        ) / 100
      );
    }
  }
};

export const schema = buildSubgraphSchema({ typeDefs, resolvers });
