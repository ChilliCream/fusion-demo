import { GraphQLError, Kind, type ValueNode } from "graphql";
import { buildSubgraphSchema } from "graphql-federation-subgraph";
import {
  isPromoCodeExpired,
  normalizePromoCode,
  promotionForProduct,
  PROMO_CODE_FORMAT,
  type CreatePromoCodeInput,
  type CreatePromotionInput,
  type PromoCode,
  type Promotion
} from "./data.js";
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
    promoCodeById(id: ID!): PromoCode @lookup @internal
  }

  type Mutation {
    "Creates a new promotion that products across the catalog can take part in."
    createPromotion(input: CreatePromotionInput!): CreatePromotionPayload! @cost(weight: "10")
    createPromoCode(input: CreatePromoCodeInput!): CreatePromoCodePayload!
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

  scalar DateTime

  input CreatePromoCodeInput {
    code: String!
    title: String!
    discountPercent: Int!
    expiresAt: DateTime
  }

  type CreatePromoCodePayload {
    promoCode: PromoCode
    errors: [CreatePromoCodeError!]
  }

  union CreatePromoCodeError =
    | PromoCodeAlreadyExistsError
    | InvalidDiscountPercentError
    | InvalidPromoCodeFormatError

  interface Error {
    message: String!
  }

  type PromoCodeAlreadyExistsError implements Error {
    message: String!
    code: String!
  }

  type InvalidDiscountPercentError implements Error {
    message: String!
    discountPercent: Int!
  }

  type InvalidPromoCodeFormatError implements Error {
    message: String!
    code: String!
  }

  type PromoCode @key(fields: "id") {
    id: ID!
    code: String!
    title: String!
    discountPercent: Int!
    expiresAt: DateTime
    isExpired: Boolean!
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

type CreatePromoCodeErrorResult =
  | { __typename: "PromoCodeAlreadyExistsError"; message: string; code: string }
  | { __typename: "InvalidDiscountPercentError"; message: string; discountPercent: number }
  | { __typename: "InvalidPromoCodeFormatError"; message: string; code: string };

interface CreatePromoCodeResult {
  promoCode: PromoCode | null;
  errors: CreatePromoCodeErrorResult[] | null;
}

function toIsoDateTime(value: string): string {
  const date = new Date(value);

  if (Number.isNaN(date.getTime())) {
    throw new GraphQLError(`"${value}" is not a valid DateTime.`);
  }

  return date.toISOString();
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
    }),
    promoCodeById: (
      _parent: unknown,
      args: { id: string },
      context: PromotionsContext
    ): Promise<PromoCode | null> => context.store.getPromoCodeById(args.id)
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
    },
    createPromoCode: async (
      _parent: unknown,
      args: { input: CreatePromoCodeInput },
      context: PromotionsContext
    ): Promise<CreatePromoCodeResult> => {
      const { title, discountPercent, expiresAt } = args.input;
      const code = normalizePromoCode(args.input.code);

      if (!PROMO_CODE_FORMAT.test(code)) {
        return {
          promoCode: null,
          errors: [
            {
              __typename: "InvalidPromoCodeFormatError",
              message: `"${code}" is not a valid promo code. Codes must be 3-32 characters of A-Z, 0-9, and -.`,
              code
            }
          ]
        };
      }

      if (discountPercent < 1 || discountPercent > 100) {
        return {
          promoCode: null,
          errors: [
            {
              __typename: "InvalidDiscountPercentError",
              message: "The discount percent must be between 1 and 100.",
              discountPercent
            }
          ]
        };
      }

      const existing = await context.store.getPromoCodeByCode(code);

      if (existing !== null) {
        return {
          promoCode: null,
          errors: [
            {
              __typename: "PromoCodeAlreadyExistsError",
              message: `A promo code "${code}" already exists.`,
              code
            }
          ]
        };
      }

      const promoCode = await context.store.createPromoCode({
        code,
        title,
        discountPercent,
        expiresAt: expiresAt ?? null
      });

      return { promoCode, errors: null };
    }
  },
  PromoCode: {
    isExpired: (parent: PromoCode): boolean => isPromoCodeExpired(parent.expiresAt)
  },
  DateTime: {
    serialize(value: unknown): string {
      if (value instanceof Date) {
        return value.toISOString();
      }

      if (typeof value === "string") {
        return toIsoDateTime(value);
      }

      throw new GraphQLError(
        `DateTime cannot represent a non-date value: ${String(value)}`
      );
    },
    parseValue(value: unknown): string {
      if (typeof value !== "string") {
        throw new GraphQLError("DateTime must be a string.");
      }

      return toIsoDateTime(value);
    },
    parseLiteral(ast: ValueNode): string {
      if (ast.kind !== Kind.STRING) {
        throw new GraphQLError("DateTime must be a string.");
      }

      return toIsoDateTime(ast.value);
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

      const cents =
        Math.round(args.price * 100) * (100 - promotion.discountPercent);
      return Math.round(cents / 100) / 100;
    }
  }
};

export const schema = buildSubgraphSchema({ typeDefs, resolvers });
