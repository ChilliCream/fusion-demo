/**
 * Cart-level money arithmetic, summed in the frontend from the server-priced
 * lines (`CartItem.unitPrice` / `CartItem.lineTotal`) and the applied
 * `Cart.promoCode`. This is the one place the sum happens - reused by every
 * cart-money task in this map, never duplicated (fusion-demo-yt-ezu).
 *
 * Cart-level totals live here instead of on the server because Fusion
 * 16.6.2 cannot aggregate a list across schemas (a requirement naming a
 * field the requiring schema owns itself doesn't compose, and a
 * list-of-scalars requirement silently returns null) - see the ruling on
 * "Revise: where are cart totals computed" (fusion-demo-yt-sry.12). Only
 * per-item money is server-computed; this module derives everything above
 * that from `lineTotal` and `promoCode`.
 */

export interface CartTotalsLineItem {
  quantity: number;
  lineTotal: number;
  product: {
    price: number;
    discountedPrice: number;
    promotion: { readonly id: string } | null | undefined;
  };
}

export interface CartTotalsPromoCode {
  discountPercent: number;
  isExpired: boolean;
}

export interface CartTotals {
  /** Sum of every line's `lineTotal`. */
  subtotal: number;
  /** `round2(subtotal * discountPercent / 100)` when an unexpired code is
   * applied, otherwise 0. */
  discount: number;
  /** `subtotal - discount`. */
  total: number;
  /** Sum of `(product.price - product.discountedPrice) * quantity` across
   * promoted lines - the existing "you save" figure, unrelated to the
   * promo code discount above. */
  savings: number;
}

/**
 * Rounds to 2 decimal places, half away from zero (so 0.005 rounds to 0.01
 * and -0.005 to -0.01), matching how a shopper expects a discount in cents
 * to round rather than the "round half to even" a plain `toFixed` can drift
 * into on some inputs.
 */
export function round2(value: number): number {
  const sign = value < 0 ? -1 : 1;
  return (sign * Math.round(Math.abs(value) * 100)) / 100;
}

/** Sums server-priced cart lines into subtotal/discount/total/savings. */
export function computeCartTotals(
  items: readonly CartTotalsLineItem[],
  promoCode: CartTotalsPromoCode | null | undefined,
): CartTotals {
  let subtotal = 0;
  let savings = 0;

  for (const item of items) {
    subtotal += item.lineTotal;
    if (item.product.promotion) {
      savings +=
        (item.product.price - item.product.discountedPrice) * item.quantity;
    }
  }

  const discount =
    promoCode && !promoCode.isExpired
      ? round2((subtotal * promoCode.discountPercent) / 100)
      : 0;

  return { subtotal, discount, total: subtotal - discount, savings };
}
