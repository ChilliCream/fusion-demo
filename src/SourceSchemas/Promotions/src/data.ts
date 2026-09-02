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

// Hashing the stable product id assigns every product the same promotion on each
// request; roughly half of the hash space maps to no promotion at all.
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
