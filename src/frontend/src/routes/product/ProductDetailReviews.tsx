import { useCallback } from "react";
import { graphql, usePaginationFragment } from "react-relay";
import type { ProductDetailReviewsPaginationQuery } from "./__generated__/ProductDetailReviewsPaginationQuery.graphql";
import type { ProductDetailReviews_product$key } from "./__generated__/ProductDetailReviews_product.graphql";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { StarRating } from "../../components/ui/StarRating";

const PAGE_SIZE = 10;

const ProductDetailReviewsFragment = graphql`
  fragment ProductDetailReviews_product on Product
  @refetchable(queryName: "ProductDetailReviewsPaginationQuery")
  @argumentDefinitions(
    count: { type: "Int", defaultValue: 10 }
    cursor: { type: "String" }
  ) {
    reviews(first: $count, after: $cursor)
      @connection(key: "ProductDetailReviews_reviews") {
      edges {
        node {
          id
          stars
          body
          createAt
          author {
            id
            username
          }
        }
      }
    }
  }
`;

function formatReviewDate(value: string): string {
  return new Date(value).toLocaleDateString("en-US", {
    month: "short",
    day: "numeric",
    year: "numeric",
  });
}

interface ProductDetailReviewsProps {
  product: ProductDetailReviews_product$key;
  /** Same 50-sample average shown in the buy rail, passed down so both
   * ratings on the page agree - see `ProductDetailView`. */
  averageRating: number;
  /** Sample-based review count (0 means "no reviews yet"); also from the
   * rail's 50-sample fragment, not this component's own paginated page. */
  reviewCount: number;
}

/**
 * The reviews section: heading + "{avg} average", then a grid of review
 * tiles (stars, body, author with an "Anonymous" fallback for a null
 * `author`, and the `createAt` date - note the schema's field name really
 * is `createAt`, not `createdAt`). `usePaginationFragment` over
 * `Product.reviews`, 10 at a time, server order (no `order` argument
 * exists). Renders the quiet "No reviews yet" panel when the first page
 * comes back empty.
 */
export function ProductDetailReviews({
  product,
  averageRating,
  reviewCount,
}: ProductDetailReviewsProps) {
  const { data, loadNext, hasNext, isLoadingNext } = usePaginationFragment<
    ProductDetailReviewsPaginationQuery,
    ProductDetailReviews_product$key
  >(ProductDetailReviewsFragment, product);

  const edges = data.reviews?.edges ?? [];

  const handleLoadMore = useCallback(() => {
    loadNext(PAGE_SIZE);
  }, [loadNext]);

  return (
    <section id="reviews" className="mt-16 scroll-mt-24">
      <div className="mb-6 flex items-baseline gap-3">
        <h2 className="font-heading text-h5 font-bold text-cc-heading">Reviews</h2>
        {reviewCount > 0 && (
          <span className="text-sm text-cc-ink-dim">
            {averageRating.toFixed(1)} average
          </span>
        )}
      </div>

      {edges.length === 0 ? (
        <Card className="mx-auto max-w-md text-center text-cc-ink-dim">
          No reviews yet.
        </Card>
      ) : (
        <>
          <div className="mb-7 grid grid-cols-[repeat(auto-fit,minmax(260px,1fr))] gap-5">
            {edges.map((edge) => (
              <Card key={edge.node.id} className="flex h-full flex-col gap-3">
                <StarRating rating={edge.node.stars} size="detail" />
                <p className="text-[0.9375rem] leading-relaxed text-cc-prose">
                  {edge.node.body}
                </p>
                <footer className="mt-auto flex items-baseline justify-between gap-4 text-[0.8125rem]">
                  <span className="font-semibold text-cc-heading">
                    {edge.node.author?.username ?? "Anonymous"}
                  </span>
                  <span className="text-cc-ink-dim">
                    {formatReviewDate(edge.node.createAt)}
                  </span>
                </footer>
              </Card>
            ))}
          </div>

          {hasNext && (
            <div className="flex justify-center">
              <Button
                variant="outline"
                size="md"
                onClick={handleLoadMore}
                disabled={isLoadingNext}
              >
                {isLoadingNext ? "Loading…" : "Load more reviews"}
              </Button>
            </div>
          )}
        </>
      )}
    </section>
  );
}
