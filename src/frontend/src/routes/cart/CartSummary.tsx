import { useState } from "react";
import { graphql, useMutation } from "react-relay";
import type { CartSummaryCheckoutMutation } from "./__generated__/CartSummaryCheckoutMutation.graphql";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";

// Re-spreads CartBadge_cart (header badge) and CartView_cart (this page's
// own data) on the checkout payload's cart, same pattern as the line item's
// mutations, so both reflect the now-emptied cart if the visitor navigates
// back to /cart later. Unlike the cart-edit mutations, `CheckoutPayload`
// has no `errors` union in the schema (only `cart`) - see this component's
// doc comment for how failure is surfaced instead.
const CheckoutMutation = graphql`
  mutation CartSummaryCheckoutMutation {
    checkout {
      cart {
        id
        ...CartBadge_cart
        ...CartView_cart
      }
    }
  }
`;

interface CartSummaryProps {
  /** Sum of every line's server-computed `lineTotal`. */
  subtotal: number;
  /** `round2(subtotal * promoCode.discountPercent / 100)` when an
   * unexpired promo code is applied, otherwise 0. */
  discount: number;
  /** `subtotal - discount`. */
  total: number;
  /** Total promo savings across all lines; 0 when nothing is discounted. */
  savings: number;
  /** Called once `Mutation.checkout` completes with no error. */
  onCheckoutSuccess: () => void;
}

/**
 * The sticky summary rail: subtotal (server line totals summed by
 * `cartTotals.ts`), a "You save" line shown only when `savings > 0`, a
 * "Discount" line shown only when `discount > 0` (the label gains the
 * applied code in a later task), the total, and the Checkout button with a
 * pending state.
 *
 * `Mutation.checkout` takes no input, and - unlike `AddProductToCartPayload`
 * / `RemoveProductFromCartPayload` - its payload (`CheckoutPayload`) has no
 * `errors` union in `src/frontend/src/schema.graphql`, only `cart`. So a
 * checkout failure can only surface here as a thrown GraphQL/network error,
 * handled via `onError`; there's no payload-level failure state to check.
 */
export function CartSummary({
  subtotal,
  discount,
  total,
  savings,
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
    commitCheckout({
      variables: {},
      onCompleted: (_response, errors) => {
        setIsCheckingOut(false);
        if (errors?.length) {
          setError("Checkout failed. Please try again.");
          return;
        }
        onCheckoutSuccess();
      },
      onError: () => {
        setIsCheckingOut(false);
        setError("Checkout failed. Please try again.");
      },
    });
  }

  return (
    <Card className="flex flex-col gap-4">
      <h2 className="font-heading text-h6 font-semibold text-cc-heading">
        Summary
      </h2>

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
          <span>Discount</span>
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
