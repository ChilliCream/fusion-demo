import { Component, Suspense, useCallback, useState, type ReactNode } from "react";
import { graphql, useLazyLoadQuery } from "react-relay";
import { useParams } from "react-router";
import type { ProductDetailPageQuery } from "./__generated__/ProductDetailPageQuery.graphql";
import { Button } from "../components/ui/Button";
import { Card } from "../components/ui/Card";
import { ProductDetailSkeleton } from "./product/ProductDetailSkeleton";
import { ProductDetailView } from "./product/ProductDetailView";
import { ProductNotFound } from "./product/ProductNotFound";

const ProductDetailPageQueryNode = graphql`
  query ProductDetailPageQuery($id: ID!) {
    productById(id: $id) {
      id
      ...ProductDetailView_product
    }
  }
`;

interface ProductDetailContentProps {
  id: string;
  /**
   * Busts Relay's `QueryResource` cache on Retry, same reasoning as
   * `OverviewPage`'s `fetchKey`: without it, a remount alone would replay
   * the same cached network error instead of re-hitting the network.
   */
  fetchKey: number;
}

/**
 * Runs `ProductDetailPageQuery` for the route's `:id` param. A `null`
 * `productById` (id doesn't resolve to a product) is not an error - it's
 * rendered as the demo-grade not-found panel rather than being thrown into
 * the error boundary below.
 */
function ProductDetailContent({ id, fetchKey }: ProductDetailContentProps) {
  const data = useLazyLoadQuery<ProductDetailPageQuery>(
    ProductDetailPageQueryNode,
    { id },
    { fetchKey },
  );

  if (!data.productById) {
    return <ProductNotFound />;
  }

  return <ProductDetailView product={data.productById} />;
}

interface ProductDetailErrorBoundaryProps {
  children: ReactNode;
  onRetry: () => void;
}

interface ProductDetailErrorBoundaryState {
  hasError: boolean;
}

/**
 * Catches the query error thrown into Suspense by `ProductDetailContent`
 * and shows the "can't be loaded" panel with a Retry pill instead. Retry is
 * wired up by an incrementing `retryKey` in `ProductDetailPage`: it remounts
 * this boundary (resetting `hasError`) and is threaded through as
 * `ProductDetailContent`'s `fetchKey`, busting Relay's `QueryResource` cache
 * so the retry actually re-hits the network. Same pattern as
 * `OverviewPage`'s `OverviewErrorBoundary`.
 */
class ProductDetailErrorBoundary extends Component<
  ProductDetailErrorBoundaryProps,
  ProductDetailErrorBoundaryState
> {
  constructor(props: ProductDetailErrorBoundaryProps) {
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
          <p className="text-cc-ink">This product can&apos;t be loaded right now</p>
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
 * The product detail route (`/products/:id`): looks the product up by its
 * opaque id via `Query.productById`, then renders the full detail layout
 * (`ProductDetailView`) - breadcrumb, media panel, buy rail, delivery
 * estimate, and paginated reviews. Loading is a route-level Suspense
 * boundary showing a layout-shaped skeleton; a thrown query error is caught
 * here and shown as a message panel with a Retry action, same as
 * `OverviewPage`.
 */
export default function ProductDetailPage() {
  const { id } = useParams<{ id: string }>();
  const [retryKey, setRetryKey] = useState(0);
  const handleRetry = useCallback(() => setRetryKey((key) => key + 1), []);

  return (
    <div className="mx-auto max-w-7xl px-6 py-16">
      {id ? (
        <ProductDetailErrorBoundary key={retryKey} onRetry={handleRetry}>
          <Suspense fallback={<ProductDetailSkeleton />}>
            <ProductDetailContent id={id} fetchKey={retryKey} />
          </Suspense>
        </ProductDetailErrorBoundary>
      ) : (
        <ProductNotFound />
      )}
    </div>
  );
}
