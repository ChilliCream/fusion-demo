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

export interface PromoCode {
  id: string;
  code: string;
  title: string;
  discountPercent: number;
  expiresAt: string | null;
}

export interface CreatePromoCodeInput {
  code: string;
  title: string;
  discountPercent: number;
  expiresAt?: string | null;
}

// Codes are matched case-insensitively and without surrounding whitespace, so
// both writes (createPromoCode) and lookups (the uniqueness check) normalize
// through this before touching the store.
export function normalizePromoCode(code: string): string {
  return code.trim().toUpperCase();
}

export const PROMO_CODE_FORMAT = /^[A-Z0-9-]{3,32}$/;

export function isPromoCodeExpired(
  expiresAt: string | null,
  now: Date = new Date()
): boolean {
  return expiresAt !== null && new Date(expiresAt) <= now;
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
