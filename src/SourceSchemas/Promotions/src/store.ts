import pg from "pg";
import type {
  CreatePromoCodeInput,
  CreatePromotionInput,
  PromoCode,
  Promotion
} from "./data.js";
import { runMigrations } from "./migrate.js";

export interface PromotionStore {
  listPromotions(): Promise<Promotion[]>;
  getPromotionById(id: string): Promise<Promotion | null>;
  createPromotion(input: CreatePromotionInput): Promise<Promotion>;
  getPromoCodeById(id: string): Promise<PromoCode | null>;
  getPromoCodeByCode(code: string): Promise<PromoCode | null>;
  createPromoCode(input: CreatePromoCodeInput): Promise<PromoCode>;
  getAppliedPromoCode(cartId: string): Promise<PromoCode | null>;
  applyPromoCode(cartId: string, promoCodeId: string): Promise<void>;
  removePromoCode(cartId: string): Promise<boolean>;
}

export async function createPromotionStore(): Promise<PromotionStore> {
  const config = databaseConfig();

  await runMigrations(config);

  const pool = new pg.Pool(config);

  // Without an 'error' listener, a dropped idle connection would crash the
  // process; the next query reconnects on its own.
  pool.on("error", (error) => {
    console.error("Idle PostgreSQL client error", error);
  });

  return new PgPromotionStore(pool);
}

function databaseConfig(): pg.PoolConfig {
  if (process.env.DATABASE_URL) {
    return { connectionString: process.env.DATABASE_URL };
  }

  // An empty config makes pg read the standard PG* variables, which is how
  // the Aspire AppHost wires the promotions database in.
  if (process.env.PGHOST) {
    return {};
  }

  throw new Error(
    "No PostgreSQL connection configured: set DATABASE_URL or the standard " +
      "PG* environment variables."
  );
}

interface PromotionRow {
  id: number;
  title: string;
  description: string | null;
  discount_percent: number;
}

interface PromoCodeRow {
  id: number;
  code: string;
  title: string;
  discount_percent: number;
  expires_at: Date | null;
}

const MAX_INT32 = 2147483647;

class PgPromotionStore implements PromotionStore {
  constructor(private readonly pool: pg.Pool) {}

  async listPromotions(): Promise<Promotion[]> {
    const result = await this.pool.query<PromotionRow>(
      "SELECT id, title, description, discount_percent FROM promotions ORDER BY id"
    );

    return result.rows.map(toPromotion);
  }

  async getPromotionById(id: string): Promise<Promotion | null> {
    const numericId = Number(id);

    // Non-integer ids cannot match and would fail the integer cast in PostgreSQL.
    if (!Number.isInteger(numericId) || numericId < 1 || numericId > MAX_INT32) {
      return null;
    }

    const result = await this.pool.query<PromotionRow>(
      "SELECT id, title, description, discount_percent FROM promotions WHERE id = $1",
      [numericId]
    );

    const row = result.rows[0];

    return row === undefined ? null : toPromotion(row);
  }

  async createPromotion(input: CreatePromotionInput): Promise<Promotion> {
    const result = await this.pool.query<PromotionRow>(
      `INSERT INTO promotions (title, description, discount_percent)
       VALUES ($1, $2, $3)
       RETURNING id, title, description, discount_percent`,
      [input.title, input.description ?? null, input.discountPercent]
    );

    const row = result.rows[0];

    if (row === undefined) {
      throw new Error("The insert into promotions returned no row.");
    }

    return toPromotion(row);
  }

  async getPromoCodeById(id: string): Promise<PromoCode | null> {
    const numericId = Number(id);

    // Non-integer ids cannot match and would fail the integer cast in PostgreSQL.
    if (!Number.isInteger(numericId) || numericId < 1 || numericId > MAX_INT32) {
      return null;
    }

    const result = await this.pool.query<PromoCodeRow>(
      "SELECT id, code, title, discount_percent, expires_at FROM promo_codes WHERE id = $1",
      [numericId]
    );

    const row = result.rows[0];

    return row === undefined ? null : toPromoCode(row);
  }

  async getPromoCodeByCode(code: string): Promise<PromoCode | null> {
    const result = await this.pool.query<PromoCodeRow>(
      "SELECT id, code, title, discount_percent, expires_at FROM promo_codes WHERE code = $1",
      [code]
    );

    const row = result.rows[0];

    return row === undefined ? null : toPromoCode(row);
  }

  async createPromoCode(input: CreatePromoCodeInput): Promise<PromoCode> {
    const result = await this.pool.query<PromoCodeRow>(
      `INSERT INTO promo_codes (code, title, discount_percent, expires_at)
       VALUES ($1, $2, $3, $4)
       RETURNING id, code, title, discount_percent, expires_at`,
      [input.code, input.title, input.discountPercent, input.expiresAt ?? null]
    );

    const row = result.rows[0];

    if (row === undefined) {
      throw new Error("The insert into promo_codes returned no row.");
    }

    return toPromoCode(row);
  }

  async getAppliedPromoCode(cartId: string): Promise<PromoCode | null> {
    const result = await this.pool.query<PromoCodeRow>(
      `SELECT pc.id, pc.code, pc.title, pc.discount_percent, pc.expires_at
       FROM cart_promo_codes cpc
       JOIN promo_codes pc ON pc.id = cpc.promo_code_id
       WHERE cpc.cart_id = $1`,
      [cartId]
    );

    const row = result.rows[0];

    return row === undefined ? null : toPromoCode(row);
  }

  async applyPromoCode(cartId: string, promoCodeId: string): Promise<void> {
    await this.pool.query(
      `INSERT INTO cart_promo_codes (cart_id, promo_code_id)
       VALUES ($1, $2)
       ON CONFLICT (cart_id) DO UPDATE
         SET promo_code_id = excluded.promo_code_id, applied_at = now()`,
      [cartId, Number(promoCodeId)]
    );
  }

  async removePromoCode(cartId: string): Promise<boolean> {
    const result = await this.pool.query(
      "DELETE FROM cart_promo_codes WHERE cart_id = $1",
      [cartId]
    );

    return (result.rowCount ?? 0) > 0;
  }
}

function toPromotion(row: PromotionRow): Promotion {
  return {
    id: String(row.id),
    title: row.title,
    description: row.description,
    discountPercent: row.discount_percent
  };
}

function toPromoCode(row: PromoCodeRow): PromoCode {
  return {
    id: String(row.id),
    code: row.code,
    title: row.title,
    discountPercent: row.discount_percent,
    expiresAt: row.expires_at === null ? null : row.expires_at.toISOString()
  };
}
