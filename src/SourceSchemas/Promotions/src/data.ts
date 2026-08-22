export interface Promotion {
  id: string;
  title: string;
  description: string | null;
  discountPercent: number;
}

export interface CreatePromotionInput {
  title: string;
  description?: string | null;
  discountPercent: number;
}

// The initial campaigns. The same rows are seeded into PostgreSQL by
// migrations/0002_seed-promotions.sql; this list only backs the in-memory
// store used when no database is configured.
export const seedPromotions: Promotion[] = [
  {
    id: "1",
    title: "Peak Season Kickoff",
    description: "Kick off the hiking season with a discount on selected gear.",
    discountPercent: 15
  },
  {
    id: "2",
    title: "Base Camp Bundle",
    description: "Stock up on camp essentials before the first ascent.",
    discountPercent: 10
  },
  {
    id: "3",
    title: "Summit Clearance",
    description: "Last season's summit gear has to go.",
    discountPercent: 30
  },
  {
    id: "4",
    title: "Trailhead Deal of the Week",
    description: null,
    discountPercent: 20
  }
];

// Promotions are assigned by hashing the stable product id, so any product resolves
// to the same promotion on every request without this service knowing the catalog.
// Roughly half of the hash space maps to no promotion at all.
export function promotionForProduct(
  productId: string,
  promotions: readonly Promotion[]
): Promotion | null {
  if (promotions.length === 0) {
    return null;
  }

  let hash = 0x811c9dc5;

  for (let i = 0; i < productId.length; i++) {
    hash ^= productId.charCodeAt(i);
    hash = Math.imul(hash, 0x01000193) >>> 0;
  }

  const slot = hash % (promotions.length * 2);

  return promotions[slot] ?? null;
}
