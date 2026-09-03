import { useState } from "react";
import { graphql, useFragment } from "react-relay";
import { Link } from "react-router";
import type { CartView_cart$key } from "./__generated__/CartView_cart.graphql";
import { Card } from "../../components/ui/Card";
import { computeCartTotals } from "./cartTotals";
import { CartLineItem } from "./CartLineItem";
import { CartSummary } from "./CartSummary";

const CartViewFragment = graphql`
  fragment CartView_cart on Cart {
    items(first: 50) {
      nodes {
        id
        quantity
        lineTotal
        product {
          price
          discountedPrice
          promotion {
            id
          }
        }
        ...CartLineItem_cartItem
      }
    }
    promoCode {
      code
      title
      discountPercent
      isExpired
    }
  }
`;

// Duplicated link markup for the outline "back to shopping" pills below
// (Card's Browse products / Continue shopping), matching ProductNotFound's
// own copy of Button's outline/md classes: Button has no react-router `Link`
// support, and a `<Link>` can't render as `href` through it either, so
// these are two extra copies of the same pill rather than editing that
// shared primitive (out of file scope for this task).
const OUTLINE_LINK_CLASSES =
  "inline-flex items-center justify-center gap-2 rounded-full border border-cc-card-border px-7 py-3 text-sm font-medium text-cc-ink no-underline transition-colors hover:border-cc-card-border-hover";

/** Checkmark glyph for the post-checkout takeover. */
function CheckCircleIcon({ className }: { className?: string }) {
  return (
    <svg
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth={1.7}
      strokeLinecap="round"
      strokeLinejoin="round"
      className={className}
      aria-hidden="true"
    >
      <circle cx="12" cy="12" r="9.5" />
      <path d="M8 12.5l2.5 2.5L16 9.5" />
    </svg>
  );
}

function CartEmptyPanel() {
  return (
    <Card className="mx-auto max-w-md text-center">
      <p className="mb-5 text-cc-ink">Your cart is empty</p>
      <Link to="/" className={OUTLINE_LINK_CLASSES}>
        Browse products
      </Link>
    </Card>
  );
}

function CartOrderPlacedPanel() {
  return (
    <Card className="mx-auto max-w-md text-center">
      <CheckCircleIcon className="mx-auto mb-4 h-14 w-14 text-cc-success" />
      <p className="mb-5 font-heading text-h6 font-semibold text-cc-heading">
        Order placed — thanks!
      </p>
      <Link to="/" className={OUTLINE_LINK_CLASSES}>
        Continue shopping
      </Link>
    </Card>
  );
}

interface CartViewProps {
  cart: CartView_cart$key;
}

/**
 * The `/cart` route's main content once signed in and loaded: a two-column
 * line-items/summary layout for a non-empty cart (stacking below ~900px,
 * same breakpoint as the product detail page), an empty-cart panel, or -
 * after a successful checkout - a full takeover confirming the order (no
 * order number; the server returns none).
 *
 * `Cart.items(first: 50)` is treated as the whole cart, an accepted
 * demo-scale limit (fusion-demo-js-p0z.5). Items are additionally filtered
 * to `quantity > 0` here: whether the server drops a cart item entirely
 * once its quantity is removed down to zero, rather than leaving a
 * zero-quantity node in the connection, hasn't been verified against the
 * live stack - this filter is defensive, and the actual server behavior is
 * a runtime check owned by fusion-demo-js-0bx.9.
 */
export function CartView({ cart }: CartViewProps) {
  const data = useFragment(CartViewFragment, cart);
  const [checkedOut, setCheckedOut] = useState(false);

  const items = (data.items?.nodes ?? []).filter(
    (node): node is NonNullable<typeof node> =>
      node !== null && node.quantity > 0,
  );

  if (checkedOut) {
    return <CartOrderPlacedPanel />;
  }

  if (items.length === 0) {
    return <CartEmptyPanel />;
  }

  const { subtotal, discount, total, savings } = computeCartTotals(
    items,
    data.promoCode,
  );

  return (
    <div>
      <h1 className="mb-8 font-heading text-h4 font-bold text-cc-heading">
        Your cart
      </h1>

      <div className="grid grid-cols-[minmax(0,3fr)_minmax(0,2fr)] items-start gap-8 max-[900px]:grid-cols-1 max-[900px]:gap-6">
        <div className="flex flex-col gap-4">
          {items.map((item) => (
            <CartLineItem key={item.id} item={item} />
          ))}
        </div>

        <div className="sticky top-24 max-[900px]:static">
          <CartSummary
            subtotal={subtotal}
            discount={discount}
            total={total}
            savings={savings}
            onCheckoutSuccess={() => setCheckedOut(true)}
          />
        </div>
      </div>
    </div>
  );
}
