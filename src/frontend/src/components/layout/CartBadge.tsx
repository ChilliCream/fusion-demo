import { Suspense, Component, type ReactNode } from "react";
import { graphql, useLazyLoadQuery, useFragment } from "react-relay";
import type { CartBadgeQuery } from "./__generated__/CartBadgeQuery.graphql";
import type { CartBadge_cart$key } from "./__generated__/CartBadge_cart.graphql";

const CartBadgeFragment = graphql`
  fragment CartBadge_cart on Cart {
    items(first: 50) {
      nodes {
        quantity
      }
    }
  }
`;

const CartBadgeQueryNode = graphql`
  query CartBadgeQuery {
    viewer {
      cart {
        ...CartBadge_cart
      }
    }
  }
`;

const MAX_DISPLAYED_COUNT = 9;

/**
 * The small pill on the header cart icon showing the sum of item quantities
 * in `viewer.cart`, capped at "9+" for display. Only mounted by `Header`
 * while the viewer is authenticated.
 *
 * Renders two pieces of text: the visible, capped-at-"9+" pill (marked
 * `aria-hidden`) and an `sr-only` ", N items" suffix carrying the real
 * count. `Header` composes its cart link's accessible name from its own
 * `sr-only` "Cart" text plus this suffix (when non-null) - see the comment
 * there for why a static `aria-label` can't be used instead.
 */
function CartBadgeCount() {
  const data = useLazyLoadQuery<CartBadgeQuery>(CartBadgeQueryNode, {});
  const cart = useFragment<CartBadge_cart$key>(
    CartBadgeFragment,
    data.viewer.cart,
  );
  const count = (cart.items?.nodes ?? []).reduce(
    (sum, node) => sum + (node?.quantity ?? 0),
    0,
  );

  if (count <= 0) {
    return null;
  }

  return (
    <>
      <span
        aria-hidden="true"
        className="absolute -top-0.5 -right-0.5 flex h-[17px] min-w-[17px] items-center justify-center rounded-full bg-cc-accent px-1 text-[11px] leading-none font-bold text-cc-surface"
      >
        {count > MAX_DISPLAYED_COUNT ? "9+" : count}
      </span>
      <span className="sr-only">
        , {count} {count === 1 ? "item" : "items"}
      </span>
    </>
  );
}

class CartBadgeErrorBoundary extends Component<
  { children: ReactNode },
  { hasError: boolean }
> {
  constructor(props: { children: ReactNode }) {
    super(props);
    this.state = { hasError: false };
  }

  static getDerivedStateFromError() {
    return { hasError: true };
  }

  render() {
    // The badge is decorative; hide it rather than break the header.
    if (this.state.hasError) {
      return null;
    }

    return this.props.children;
  }
}

export function CartBadge() {
  return (
    <CartBadgeErrorBoundary>
      <Suspense fallback={null}>
        <CartBadgeCount />
      </Suspense>
    </CartBadgeErrorBoundary>
  );
}
