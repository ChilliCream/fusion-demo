import {
  useEffect,
  useRef,
  useState,
  type MouseEvent as ReactMouseEvent,
} from "react";
import { graphql, useFragment, useMutation } from "react-relay";
import { Link } from "react-router";
import type { ProductDetailView_product$key } from "./__generated__/ProductDetailView_product.graphql";
import type { ProductDetailViewAddToCartMutation } from "./__generated__/ProductDetailViewAddToCartMutation.graphql";
import { useAuth } from "../../auth/useAuth";
import { Button } from "../../components/ui/Button";
import { StarRating } from "../../components/ui/StarRating";
import { DeliveryEstimateCard } from "./DeliveryEstimateCard";
import { ProductDetailReviews } from "./ProductDetailReviews";

const ProductDetailViewFragment = graphql`
  fragment ProductDetailView_product on Product {
    id
    name
    pictureUrl
    price
    discountedPrice
    promotion {
      discountPercent
    }
    reviewSummary: reviews(first: 50) {
      nodes {
        stars
      }
      pageInfo {
        hasNextPage
      }
    }
    ...ProductDetailReviews_product
  }
`;

// Re-spreads CartBadge_cart (defined in components/layout/CartBadge.tsx) on
// the mutation payload's cart, same as OverviewProductCard's and
// AuthProvider's adds, so the header badge count updates from this
// mutation too.
const AddToCartMutation = graphql`
  mutation ProductDetailViewAddToCartMutation($input: AddProductToCartInput!) {
    addProductToCart(input: $input) {
      cart {
        id
        ...CartBadge_cart
      }
    }
  }
`;

const ADDED_RESET_DELAY_MS = 2000;

/** Simple "no picture" glyph, ported from the prototype's `#img-none`
 * symbol - duplicated locally rather than shared with
 * `OverviewProductCard` (out of file scope for this task). */
function NoImageIcon({ className }: { className?: string }) {
  return (
    <svg
      viewBox="0 0 64 64"
      fill="none"
      stroke="currentColor"
      strokeWidth={3}
      className={className}
      aria-hidden="true"
    >
      <rect x="8" y="12" width="48" height="40" rx="6" />
      <circle cx="24" cy="26" r="4" fill="currentColor" stroke="none" />
      <path
        d="M14 44l12-12 8 8 8-9 8 13"
        strokeLinejoin="round"
        strokeLinecap="round"
      />
    </svg>
  );
}

interface ProductDetailViewProps {
  product: ProductDetailView_product$key;
}

/**
 * The full `/products/:id` layout once the product has loaded: breadcrumb,
 * a 60/40 media/rail grid (stacking below ~900px), and the reviews section.
 *
 * Star average and review count in the rail come from the fragment's
 * aliased `reviewSummary: reviews(first: 50)` sample - the same 50-sample
 * average logic as `OverviewProductCard` - and are passed down to
 * `ProductDetailReviews` so its "{avg} average" heading agrees with the
 * rail. The actual review tiles come from that component's own paginated
 * `Product.reviews` connection (10 per page). This component never selects
 * `Product.error` (fault-injection field, out of scope here).
 */
export function ProductDetailView({ product }: ProductDetailViewProps) {
  const data = useFragment(ProductDetailViewFragment, product);
  const { isAuthenticated, openLogin } = useAuth();
  const [commitAddToCart] = useMutation<ProductDetailViewAddToCartMutation>(
    AddToCartMutation,
  );

  const [quantity, setQuantity] = useState(1);
  const [isAdding, setIsAdding] = useState(false);
  const [isAdded, setIsAdded] = useState(false);
  const addedResetTimeoutRef = useRef<ReturnType<typeof setTimeout> | null>(
    null,
  );

  // Cleanup the transient "Added ✓" timeout on unmount so it never fires a
  // state update against an unmounted component.
  useEffect(() => {
    return () => {
      if (addedResetTimeoutRef.current !== null) {
        clearTimeout(addedResetTimeoutRef.current);
      }
    };
  }, []);

  const reviewNodes = data.reviewSummary?.nodes ?? [];
  const reviewCount = reviewNodes.length;
  const averageRating =
    reviewCount > 0
      ? Math.round(
          (reviewNodes.reduce((sum, node) => sum + node.stars, 0) /
            reviewCount) *
            10,
        ) / 10
      : 0;
  const hasMoreReviews = data.reviewSummary?.pageInfo.hasNextPage ?? false;

  function handleAddToCart() {
    if (isAdding) {
      return;
    }

    if (!isAuthenticated) {
      openLogin({ productId: data.id, quantity });
      return;
    }

    setIsAdding(true);
    commitAddToCart({
      variables: { input: { productId: data.id, quantity } },
      onCompleted: (_response, errors) => {
        setIsAdding(false);

        // Transport-level errors (Relay's second `onCompleted` argument) mean
        // the add didn't actually happen server-side, so this must not show
        // the "Added ✓" state - same as a thrown `onError`. There's no error
        // UI here to surface it further (out of scope here).
        if (errors?.length) {
          return;
        }

        setIsAdded(true);
        setQuantity(1);

        if (addedResetTimeoutRef.current !== null) {
          clearTimeout(addedResetTimeoutRef.current);
        }
        addedResetTimeoutRef.current = setTimeout(() => {
          setIsAdded(false);
          addedResetTimeoutRef.current = null;
        }, ADDED_RESET_DELAY_MS);
      },
      onError: () => {
        setIsAdding(false);
      },
    });
  }

  function handleScrollToReviews(event: ReactMouseEvent<HTMLAnchorElement>) {
    event.preventDefault();
    document.getElementById("reviews")?.scrollIntoView({ behavior: "smooth" });
  }

  return (
    <div>
      <Link
        to="/"
        className="mb-8 inline-flex items-center gap-2 text-sm text-cc-accent no-underline hover:text-cc-accent-hover"
      >
        ← Back to all products
      </Link>

      <div className="grid grid-cols-[minmax(0,3fr)_minmax(0,2fr)] items-start gap-11 max-[900px]:grid-cols-1 max-[900px]:gap-7">
        <div className="relative aspect-[4/3] overflow-hidden rounded-2xl border border-cc-card-border bg-cc-card-bg backdrop-blur-sm">
          <div
            aria-hidden="true"
            className="pointer-events-none absolute -top-24 right-0 z-0 h-56 w-56 opacity-40 blur-3xl"
            style={{
              background:
                "radial-gradient(50% 50% at 60% 40%, rgba(22,185,228,0.18), transparent 70%)",
            }}
          />
          <div
            className="relative z-10 flex h-full w-full items-center justify-center"
            style={{
              background:
                "radial-gradient(120% 90% at 50% 0%, rgba(22,185,228,0.07), transparent 60%), rgba(245,241,234,0.03)",
            }}
          >
            {data.pictureUrl ? (
              <img
                src={data.pictureUrl}
                alt={data.name}
                className="h-full w-full object-cover"
              />
            ) : (
              <div className="flex flex-col items-center gap-2 text-cc-ink-faint">
                <NoImageIcon className="h-14 w-14" />
                <span className="text-xs text-cc-ink-dim opacity-75">
                  Image coming soon
                </span>
              </div>
            )}
          </div>
        </div>

        <div>
          <h1 className="mb-3 font-heading text-h3 font-semibold text-cc-heading max-[900px]:text-h4">
            {data.name}
          </h1>

          <div className="mb-6 flex items-center gap-2 text-sm text-cc-ink-dim">
            <StarRating rating={averageRating} size="detail" />
            {reviewCount > 0 ? (
              <span>
                {averageRating.toFixed(1)} ·{" "}
                <a
                  href="#reviews"
                  onClick={handleScrollToReviews}
                  className="text-cc-accent no-underline hover:text-cc-accent-hover"
                >
                  {hasMoreReviews ? "50+" : reviewCount} reviews
                </a>
              </span>
            ) : (
              <span>No reviews yet</span>
            )}
          </div>

          <div className="mb-7 flex flex-wrap items-center gap-3">
            {data.promotion ? (
              <>
                <span className="font-heading text-h4 font-bold text-cc-heading">
                  ${data.discountedPrice.toFixed(2)}
                </span>
                <span className="text-base text-cc-ink-dim line-through">
                  ${data.price.toFixed(2)}
                </span>
                <span className="inline-flex items-center rounded-full border border-cc-danger/35 bg-cc-danger/10 px-2.5 py-0.5 text-xs font-bold tracking-wide text-cc-danger">
                  −{data.promotion.discountPercent}%
                </span>
              </>
            ) : (
              <span className="font-heading text-h4 font-bold text-cc-heading">
                ${data.price.toFixed(2)}
              </span>
            )}
          </div>

          <div className="flex flex-wrap items-center gap-4">
            <div
              className="inline-flex items-center rounded-full border border-cc-card-border"
              aria-label="Quantity"
            >
              <button
                type="button"
                aria-label="Decrease quantity"
                disabled={quantity <= 1}
                className="flex h-11 w-11 items-center justify-center rounded-full text-lg text-cc-heading transition-colors hover:bg-cc-hover disabled:cursor-not-allowed disabled:opacity-40"
                onClick={() => setQuantity((current) => Math.max(1, current - 1))}
              >
                −
              </button>
              <output
                aria-live="polite"
                className="min-w-[2.25rem] text-center font-heading text-base font-semibold text-cc-heading"
              >
                {quantity}
              </output>
              <button
                type="button"
                aria-label="Increase quantity"
                className="flex h-11 w-11 items-center justify-center rounded-full text-lg text-cc-heading transition-colors hover:bg-cc-hover"
                onClick={() => setQuantity((current) => current + 1)}
              >
                +
              </button>
            </div>

            <Button
              variant="solid"
              size="md"
              disabled={isAdding}
              onClick={handleAddToCart}
            >
              {isAdded ? "Added ✓" : isAdding ? "Adding…" : "Add to cart"}
            </Button>
          </div>

          <DeliveryEstimateCard productId={data.id} />
        </div>
      </div>

      <ProductDetailReviews
        product={data}
        averageRating={averageRating}
        reviewCount={reviewCount}
      />
    </div>
  );
}
