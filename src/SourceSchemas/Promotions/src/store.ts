import pg from "pg";
import type { CreatePromotionInput, Promotion } from "./data.js";
import { runMigrations } from "./migrate.js";

export interface PromotionStore {
  listPromotions(): Promise<Promotion[]>;
  getPromotionById(id: string): Promise<Promotion | null>;
  createPromotion(input: CreatePromotionInput): Promise<Promotion>;
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
}

function toPromotion(row: PromotionRow): Promotion {
  return {
    id: String(row.id),
    title: row.title,
    description: row.description,
    discountPercent: row.discount_percent
  };
}
