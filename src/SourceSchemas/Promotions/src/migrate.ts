import { fileURLToPath } from "node:url";
import { runner } from "node-pg-migrate";
import type pg from "pg";

// Resolves to the checked-in migrations folder both when running from src
// (tsx) and from dist (production build).
const migrationsDir = fileURLToPath(new URL("../migrations", import.meta.url));

// Mirrors the .NET source schemas, which apply their EF migrations on startup.
export async function runMigrations(config: pg.ClientConfig): Promise<void> {
  await runner({
    databaseUrl: config,
    dir: migrationsDir,
    direction: "up",
    migrationsTable: "pgmigrations",
    // Concurrent starters wait on the advisory lock and then no-op instead
    // of failing, matching how the EF migration lock behaves in the .NET
    // source schemas.
    advisoryLockMode: "wait"
  });
}
