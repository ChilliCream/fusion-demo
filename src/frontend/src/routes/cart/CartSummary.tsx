import { useState } from "react";
import { graphql, useMutation } from "react-relay";
import type { CartSummaryCheckoutMutation } from "./__generated__/CartSummaryCheckoutMutation.graphql";
import type { CartPromoCode_cart$key } from "./__generated__/CartPromoCode_cart.graphql";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { CartPromoCode } from "./CartPromoCode";

// Re-spreads CartBadge_cart (header badge) and CartView_cart (this page's
// own data) on the checkout payload's cart, same pattern as the line item's
// mutations, so both reflect the now-emptied cart if the visitor navigates
// back to /cart later (a fresh cart with a new id - checkout deletes the
// old one). Selects the payload's `errors` union, same pattern as
// CartPromoCode's mutations: a non-empty list is a payload-level failure
// (`CartIsEmptyError`), never a thrown error.
const CheckoutMutation = graphql`
  mutation CartSummaryCheckoutMutation {
    checkout {
      cart {
        id
        ...CartBadge_cart
        ...CartView_cart
      }
      errors {
        __typename
        ... on Error {
          message
        }
      }
    }
  }
`;

const GENERIC_ERROR_MESSAGE = "Something went wrong. Please try again.";

/** The receipt shown on the post-checkout panel: values read off the
 * already-rendered summary at the moment Checkout is pressed, per the
 * design ruling (fusion-demo-yt-sry.12) that `CheckoutPayload` carries no
 * totals - the receipt repeats what the shopper last saw, not a
 * server-confirmed figure. */
export interface CheckoutReceipt {
  /** The applied promo code's `code`, or `null` when none was applied. */
  promoCode: { code: string } | null;
  /** `0` when no code was applied. */
  discount: number;
  total: number;
}

interface CartSummaryProps {
  /** Sum of every line's `lineTotal(unitPrice, quantity)` (see cartTotals.ts). */
  subtotal: number;
  /** `round2(subtotal * promoCode.discountPercent / 100)` when an
   * unexpired promo code is applied, otherwise 0. */
  discount: number;
  /** `subtotal - discount`. */
  total: number;
  /** Total promo savings across all lines; 0 when nothing is discounted. */
  savings: number;
  /** The applied promo code's `code`, used to label the Discount line
   * ("Discount (SAVE10)"); `null` when no code is applied. */
  promoCode: { code: string } | null;
  /** Fragment key for the promo code row's own `Cart.id` + `promoCode`. */
  cart: CartPromoCode_cart$key;
  /** Called once `Mutation.checkout` completes with no payload error, with
   * the receipt captured from this render's own totals. */
  onCheckoutSuccess: (receipt: CheckoutReceipt) => void;
}

/**
 * The sticky summary rail: the `CartPromoCode` row, subtotal (server line
 * totals summed by `cartTotals.ts`), a "You save" line shown only when
 * `savings > 0`, a "Discount (CODE)" line shown only when `discount > 0`,
 * the total, and the Checkout button with a pending state.
 *
 * `Mutation.checkout` takes no input. Its payload (`CheckoutPayload`)
 * carries `errors: [CheckoutError!]` (currently just `CartIsEmptyError`),
 * checked the same way as `CartPromoCode`'s mutations: a non-empty list is
 * shown inline here, never thrown. `onError` (transport/GraphQL failure)
 * falls back to the same generic copy.
 */
export function CartSummary({
  subtotal,
  discount,
  total,
  savings,
  promoCode,
  cart,
  onCheckoutSuccess,
}: CartSummaryProps) {
  const [commitCheckout] =
    useMutation<CartSummaryCheckoutMutation>(CheckoutMutation);
  const [isCheckingOut, setIsCheckingOut] = useState(false);
  const [error, setError] = useState<string | null>(null);

  function handleCheckout() {
    if (isCheckingOut) {
      return;
    }
    setIsCheckingOut(true);
    setError(null);
    // Captured now, before the mutation commits: the receipt is the
    // pre-checkout values this summary is already showing, not anything
    // the payload returns (fusion-demo-yt-sry.12).
    const receipt: CheckoutReceipt = { promoCode, discount, total };
    commitCheckout({
      variables: {},
      onCompleted: (response, transportErrors) => {
        setIsCheckingOut(false);
        if (transportErrors?.length) {
          setError(GENERIC_ERROR_MESSAGE);
          return;
        }
        const payloadErrors = response.checkout.errors ?? [];
        if (payloadErrors.length > 0) {
          setError(payloadErrors[0].message ?? GENERIC_ERROR_MESSAGE);
          return;
        }
        onCheckoutSuccess(receipt);
      },
      onError: () => {
        setIsCheckingOut(false);
        setError(GENERIC_ERROR_MESSAGE);
      },
    });
  }

  return (
    <Card className="flex flex-col gap-4">
      <h2 className="font-heading text-h6 font-semibold text-cc-heading">
        Summary
      </h2>

      <CartPromoCode cart={cart} />

      <div className="flex items-center justify-between text-sm text-cc-ink-dim">
        <span>Subtotal</span>
        <span className="text-cc-heading">${subtotal.toFixed(2)}</span>
      </div>

      {savings > 0 && (
        <div className="flex items-center justify-between text-sm text-cc-success">
          <span>You save</span>
          <span>${savings.toFixed(2)}</span>
        </div>
      )}

      {discount > 0 && (
        <div className="flex items-center justify-between text-sm text-cc-success">
          <span>Discount{promoCode ? ` (${promoCode.code})` : ""}</span>
          <span>-${discount.toFixed(2)}</span>
        </div>
      )}

      <div className="flex items-center justify-between border-t border-cc-card-border pt-4 font-heading text-base font-semibold text-cc-heading">
        <span>Total</span>
        <span>${total.toFixed(2)}</span>
      </div>

      {error && (
        <p role="alert" className="text-sm text-cc-danger">
          {error}
        </p>
      )}

      <Button
        variant="solid"
        size="md"
        disabled={isCheckingOut}
        onClick={handleCheckout}
        className="w-full"
      >
        {isCheckingOut ? "Placing order…" : "Checkout"}
      </Button>
    </Card>
  );
}
