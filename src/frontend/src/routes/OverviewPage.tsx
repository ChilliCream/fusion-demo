import { Component, Suspense, useCallback, useState, type ReactNode } from "react";
import { graphql, useLazyLoadQuery } from "react-relay";
import type { OverviewPageQuery } from "./__generated__/OverviewPageQuery.graphql";
import { Button } from "../components/ui/Button";
import { Card } from "../components/ui/Card";
import { ProductGrid } from "./overview/OverviewProductGrid";
import { ProductGridSkeleton } from "./overview/ProductGridSkeleton";

const OverviewPageQueryNode = graphql`
  query OverviewPageQuery {
    ...OverviewProductGrid_query @arguments(count: 12)
  }
`;

interface OverviewContentProps {
  /**
   * Busts Relay's `QueryResource` cache on Retry. Without a changing
   * `fetchKey`, remounting alone replays the same cached network error
   * instead of issuing a fresh request for the identical (variable-less)
   * query.
   */
  fetchKey: number;
}

function OverviewContent({ fetchKey }: OverviewContentProps) {
  const data = useLazyLoadQuery<OverviewPageQuery>(
    OverviewPageQueryNode,
    {},
    { fetchKey, fetchPolicy: "network-only" },
  );
  return <ProductGrid queryRef={data} />;
}

interface OverviewErrorBoundaryProps {
  children: ReactNode;
  onRetry: () => void;
}

interface OverviewErrorBoundaryState {
  hasError: boolean;
}

/**
 * Catches the query error thrown into Suspense by `OverviewContent` and
 * shows the "can't be loaded" panel with a Retry pill instead. Retry is
 * wired up by an incrementing `retryKey` in `OverviewPage`: it remounts
 * this boundary (resetting `hasError`) and is threaded through as
 * `OverviewContent`'s `fetchKey`, which busts Relay's `QueryResource` cache
 * so the retry actually re-hits the network instead of replaying the same
 * cached error for the identical query.
 */
class OverviewErrorBoundary extends Component<
  OverviewErrorBoundaryProps,
  OverviewErrorBoundaryState
> {
  constructor(props: OverviewErrorBoundaryProps) {
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
          <p className="text-cc-ink">Products can't be loaded right now</p>
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
 * The product overview route (`/`): the first page of `Query.products` (12
 * at a time, paginated by `ProductGrid`) laid out in the website's Card
 * tile grid. Loading is a route-level Suspense boundary showing skeleton
 * tiles; a thrown query error is caught here and shown as a message panel
 * with a Retry action.
 */
export default function OverviewPage() {
  const [retryKey, setRetryKey] = useState(0);
  const handleRetry = useCallback(() => setRetryKey((key) => key + 1), []);

  return (
    <div className="mx-auto max-w-7xl px-6 py-16">
      <header className="mb-10 max-w-2xl">
        <h1 className="mb-2 font-heading text-h4 font-bold text-cc-heading">
          All products
        </h1>
        <p className="text-cc-ink-dim">
          Everything the store currently has in stock.
        </p>
      </header>

      <OverviewErrorBoundary key={retryKey} onRetry={handleRetry}>
        <Suspense fallback={<ProductGridSkeleton />}>
          <OverviewContent fetchKey={retryKey} />
        </Suspense>
      </OverviewErrorBoundary>
    </div>
  );
}
