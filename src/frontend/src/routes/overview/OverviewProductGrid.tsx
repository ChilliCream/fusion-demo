import { useCallback } from "react";
import { graphql, usePaginationFragment } from "react-relay";
import type { OverviewProductGridPaginationQuery } from "./__generated__/OverviewProductGridPaginationQuery.graphql";
import type { OverviewProductGrid_query$key } from "./__generated__/OverviewProductGrid_query.graphql";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { ProductCard } from "./OverviewProductCard";

/** Shared by the real grid and its loading skeleton so the two line up. */
export const OVERVIEW_GRID_CLASSES =
  "grid grid-cols-[repeat(auto-fill,minmax(270px,1fr))] gap-6";

const PAGE_SIZE = 12;

const ProductGridFragment = graphql`
  fragment OverviewProductGrid_query on Query
  @refetchable(queryName: "OverviewProductGridPaginationQuery")
  @argumentDefinitions(
    count: { type: "Int", defaultValue: 12 }
    cursor: { type: "String" }
  ) {
    products(first: $count, after: $cursor)
      @connection(key: "OverviewProductGrid_products") {
      edges {
        node {
          id
          ...OverviewProductCard_product
        }
      }
    }
  }
`;

interface ProductGridProps {
  queryRef: OverviewProductGrid_query$key;
}

/**
 * The product grid itself: `usePaginationFragment` over `Query.products`,
 * an outline "Load more" pill that fetches 12 more, hidden once there's no
 * next page. Renders the quiet "No products yet" panel when the first page
 * comes back empty.
 */
export function ProductGrid({ queryRef }: ProductGridProps) {
  const { data, loadNext, hasNext, isLoadingNext } = usePaginationFragment<
    OverviewProductGridPaginationQuery,
    OverviewProductGrid_query$key
  >(ProductGridFragment, queryRef);

  const edges = data.products?.edges ?? [];

  const handleLoadMore = useCallback(() => {
    loadNext(PAGE_SIZE);
  }, [loadNext]);

  if (edges.length === 0) {
    return (
      <Card className="mx-auto max-w-md text-center text-cc-ink-dim">
        No products yet.
      </Card>
    );
  }

  return (
    <>
      <div className={OVERVIEW_GRID_CLASSES}>
        {edges.map((edge) => (
          <ProductCard key={edge.node.id} product={edge.node} />
        ))}
      </div>
      {hasNext && (
        <div className="mt-10 flex justify-center">
          <Button
            variant="outline"
            size="md"
            onClick={handleLoadMore}
            disabled={isLoadingNext}
          >
            {isLoadingNext ? "Loading…" : "Load more"}
          </Button>
        </div>
      )}
    </>
  );
}
