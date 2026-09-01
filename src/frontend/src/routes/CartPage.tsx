import { Component, Suspense, useCallback, useState, type ReactNode } from "react";
import { graphql, useLazyLoadQuery } from "react-relay";
import type { CartPageQuery } from "./__generated__/CartPageQuery.graphql";
import { useAuth } from "../auth/useAuth";
import { Button } from "../components/ui/Button";
import { Card } from "../components/ui/Card";
import { CartSignInPanel } from "./cart/CartSignInPanel";
import { CartSkeleton } from "./cart/CartSkeleton";
import { CartView } from "./cart/CartView";

const CartPageQueryNode = graphql`
  query CartPageQuery {
    viewer {
      cart {
        ...CartView_cart
      }
    }
  }
`;

interface CartContentProps {
  /**
   * Busts Relay's `QueryResource` cache on Retry, same reasoning as
   * `OverviewPage`'s and `ProductDetailPage`'s `fetchKey`: without it, a
   * remount alone would replay the same cached network error instead of
   * re-hitting the network.
   */
  fetchKey: number;
}

function CartContent({ fetchKey }: CartContentProps) {
  const data = useLazyLoadQuery<CartPageQuery>(
    CartPageQueryNode,
    {},
    { fetchKey, fetchPolicy: "network-only" },
  );
  return <CartView cart={data.viewer.cart} />;
}

interface CartErrorBoundaryProps {
  children: ReactNode;
  onRetry: () => void;
}

interface CartErrorBoundaryState {
  hasError: boolean;
}

/**
 * Catches the query error thrown into Suspense by `CartContent` and shows
 * the "can't be loaded" panel with a Retry pill instead. Same pattern as
 * `OverviewPage`'s `OverviewErrorBoundary` and `ProductDetailPage`'s
 * `ProductDetailErrorBoundary`: an incrementing `retryKey` in `CartPage`
 * remounts this boundary (resetting `hasError`) and is threaded through as
 * `CartContent`'s `fetchKey`.
 */
class CartErrorBoundary extends Component<
  CartErrorBoundaryProps,
  CartErrorBoundaryState
> {
  constructor(props: CartErrorBoundaryProps) {
    super(props);
    this.state = { hasError: false };
  }

  static getDerivedStateFromError() {
    return { hasError: true };
  }

  render() {
    if (this.state.hasError) {
      return (
        <Card className="mx-auto max-w-md text-center">
          <p className="text-cc-ink">Your cart can&apos;t be loaded right now</p>
          <Button
            variant="outline"
            size="md"
            className="mt-5"
            onClick={this.props.onRetry}
          >
            Retry
          </Button>
        </Card>
      );
    }

    return this.props.children;
  }
}

/**
 * The cart route (`/cart`): `viewer.cart.items(first: 50)`, treated as the
 * whole cart (an accepted demo-scale limit, see fusion-demo-js-p0z.5).
 *
 * `viewer.cart` is an authenticated field, so signed out this renders
 * `CartSignInPanel` instead of ever mounting the query - same gating
 * `Header`'s `CartBadge` uses for its own cart query. Once signed in (via
 * the shared login modal), this component re-renders and the live query
 * takes over in place, no extra plumbing needed. Loading is a route-level
 * Suspense boundary showing a layout-shaped skeleton; a thrown query error
 * is caught here and shown as a message panel with a Retry action, same as
 * `OverviewPage` and `ProductDetailPage`.
 */
export default function CartPage() {
  const { isAuthenticated } = useAuth();
  const [retryKey, setRetryKey] = useState(0);
  const handleRetry = useCallback(() => setRetryKey((key) => key + 1), []);

  return (
    <div className="mx-auto max-w-7xl px-6 py-16">
      {isAuthenticated ? (
        <CartErrorBoundary key={retryKey} onRetry={handleRetry}>
          <Suspense fallback={<CartSkeleton />}>
            <CartContent fetchKey={retryKey} />
          </Suspense>
        </CartErrorBoundary>
      ) : (
        <CartSignInPanel />
      )}
    </div>
  );
}
