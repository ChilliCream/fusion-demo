import { useEffect, useRef, useState } from "react";
import { graphql, useFragment, useMutation } from "react-relay";
import { Link } from "react-router";
import type { OverviewProductCard_product$key } from "./__generated__/OverviewProductCard_product.graphql";
import type { OverviewProductCardAddToCartMutation } from "./__generated__/OverviewProductCardAddToCartMutation.graphql";
import { useAuth } from "../../auth/useAuth";
import { Button } from "../../components/ui/Button";
import { StarRating } from "../../components/ui/StarRating";

const ProductCardFragment = graphql`
  fragment OverviewProductCard_product on Product {
    id
    name
    price
    pictureUrl
    discountedPrice
    promotion {
      discountPercent
    }
    reviews(first: 50) {
      nodes {
        stars
      }
      pageInfo {
        hasNextPage
      }
    }
  }
`;

// Re-spreads CartBadge_cart (defined in components/layout/CartBadge.tsx) on
// the mutation payload's cart, exactly like AuthProvider.tsx's replayed add,
// so the header badge count updates from this mutation too.
const AddToCartMutation = graphql`
  mutation OverviewProductCardAddToCartMutation(
    $input: AddProductToCartInput!
  ) {
    addProductToCart(input: $input) {
      cart {
        id
        ...CartBadge_cart
      }
    }
  }
`;

interface ProductCardProps {
  product: OverviewProductCard_product$key;
}

const ADDED_RESET_DELAY_MS = 2000;

/** Simple "no picture" glyph, ported from the prototype's `#img-none` symbol. */
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

/**
 * The website's Card tile look applied to a `Product`: 16:10 media (picture
 * or an "Image coming soon" placeholder), a cyan glow that appears on
 * hover, and a hover-revealed "Add to cart" pill. The tile follows the
 * "stretched link" pattern rather than wrapping the whole tile (including
 * the Add-to-cart `<button>`) in a `react-router` `Link`, which would nest
 * interactive content inside an `<a>` (invalid HTML): the `<article>` root
 * is `relative`/`isolate`, a `Link` overlay (`absolute inset-0`) stretches
 * over it carrying the client-side navigation and the tile's accessible
 * name (via `aria-label`, since the overlay itself renders no visible
 * content), and the Add-to-cart button is a sibling stacked above that
 * overlay via z-index so its clicks never reach the anchor. The shared
 * `Card`/`Button` primitives only render plain `<a>`/`<button>` elements
 * (no `Link` support yet), so this component builds its own tile markup
 * rather than editing those primitives. It intentionally doesn't reuse the
 * `Card` component: `Card`'s fixed `p-6` padding fights a media area that
 * has to bleed to the tile's edges, and overriding it via `className` is
 * unreliable (Tailwind utility collisions between a primitive's base
 * classes and caller overrides don't reliably resolve in the caller's
 * favor).
 *
 * Star average and review count are computed client-side from the fragment's
 * `reviews(first: 50)` sample; `pageInfo.hasNextPage` on that same sample is
 * what flips the caption to "50+ reviews" rather than an exact count. This
 * component never selects `Product.error` (fault-injection field, out of
 * scope here).
 */
export function ProductCard({ product }: ProductCardProps) {
  const data = useFragment(ProductCardFragment, product);
  const { isAuthenticated, openLogin } = useAuth();
  const [commitAddToCart] = useMutation<OverviewProductCardAddToCartMutation>(
    AddToCartMutation,
  );
  const [isAdding, setIsAdding] = useState(false);
  const [isAdded, setIsAdded] = useState(false);
  const addedResetTimeoutRef = useRef<ReturnType<typeof setTimeout> | null>(
    null,
  );

  // Cleanup the transient "Added ✓" timeout on unmount so it never fires a
  // state update against an unmounted component (same pattern as
  // ProductDetailView's own add-to-cart timeout).
  useEffect(() => {
    return () => {
      if (addedResetTimeoutRef.current !== null) {
        clearTimeout(addedResetTimeoutRef.current);
      }
    };
  }, []);

  const reviewNodes = data.reviews?.nodes ?? [];
  const reviewCount = reviewNodes.length;
  const averageRating =
    reviewCount > 0
      ? Math.round(
          (reviewNodes.reduce((sum, node) => sum + node.stars, 0) /
            reviewCount) *
            10,
        ) / 10
      : 0;
  const hasMoreReviews = data.reviews?.pageInfo.hasNextPage ?? false;

  function handleAddToCart() {
    if (isAdding || isAdded) {
      return;
    }

    if (!isAuthenticated) {
      openLogin({ productId: data.id, quantity: 1 });
      return;
    }

    setIsAdding(true);
    commitAddToCart({
      variables: { input: { productId: data.id, quantity: 1 } },
      onCompleted: () => {
        setIsAdding(false);
        setIsAdded(true);

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

  return (
    <article className="group relative isolate flex h-full flex-col overflow-hidden rounded-xl border border-cc-card-border bg-cc-card-bg backdrop-blur-sm transition-colors hover:border-cc-card-border-hover">
      {/*
        Stretched-link overlay: covers the whole tile and carries the
        client-side navigation + the tile's accessible name (it renders no
        visible content, so it needs its own aria-label). It sits at z-20,
        below the Add-to-cart button's z-30 wrapper, so a click on the
        button always hits the button and never this overlay - no
        stopPropagation/preventDefault needed to keep the two from
        conflicting. It sits above the z-10 badge/copy layers so a click
        anywhere else on the tile still navigates.
      */}
      <Link
        to={`/products/${data.id}`}
        aria-label={data.name}
        className="absolute inset-0 z-20 rounded-xl focus:outline-none focus-visible:ring-2 focus-visible:ring-cc-accent"
      />

      <div
        aria-hidden="true"
        className="pointer-events-none absolute -top-24 right-0 z-0 h-56 w-56 opacity-0 blur-3xl transition-opacity duration-300 group-hover:opacity-40"
        style={{
          background:
            "radial-gradient(50% 50% at 60% 40%, rgba(22,185,228,0.18), transparent 70%)",
        }}
      />

      <div
        className="relative aspect-[16/10] w-full overflow-hidden border-b border-cc-card-border"
        style={{
          background:
            "radial-gradient(120% 90% at 50% 0%, rgba(22,185,228,0.07), transparent 60%), rgba(245,241,234,0.03)",
        }}
      >
        {data.promotion && (
          <span className="absolute top-3 left-3 z-10 inline-flex items-center rounded-full border border-cc-danger/35 bg-cc-danger/10 px-2.5 py-0.5 text-xs font-bold tracking-wide text-cc-danger">
            −{data.promotion.discountPercent}%
          </span>
        )}

        {data.pictureUrl ? (
          <img
            src={data.pictureUrl}
            alt={data.name}
            className="h-full w-full object-cover"
          />
        ) : (
          <div className="flex h-full w-full flex-col items-center justify-center gap-2 text-cc-ink-faint">
            <NoImageIcon className="h-11 w-11" />
            <span className="text-xs text-cc-ink-dim opacity-75">
              Image coming soon
            </span>
          </div>
        )}

        {/*
          Sibling of the Link overlay above (not nested inside it - an
          interactive <button> inside an <a> is invalid HTML), stacked above
          it via z-30 so a click here always lands on the button and never
          reaches the overlay underneath.
        */}
        <div className="absolute right-3 bottom-3 z-30 opacity-0 transition-opacity duration-200 group-hover:opacity-100 group-focus-within:opacity-100">
          <Button
            variant="solid"
            size="sm"
            disabled={isAdding || isAdded}
            onClick={handleAddToCart}
          >
            {isAdded ? "Added ✓" : isAdding ? "Adding…" : "Add to cart"}
          </Button>
        </div>
      </div>

      <div className="relative z-10 flex flex-1 flex-col px-5 pt-[1.1rem] pb-[1.3rem]">
        <h2 className="mb-1.5 truncate font-heading text-h6 font-semibold text-cc-heading">
          {data.name}
        </h2>

        <div className="mb-3 flex items-center gap-2 text-[0.8125rem] text-cc-ink-dim">
          <StarRating rating={averageRating} size="card" />
          <span>
            {reviewCount > 0
              ? `${averageRating.toFixed(1)} · ${hasMoreReviews ? "50+" : reviewCount} reviews`
              : "No reviews yet"}
          </span>
        </div>

        <div className="mt-auto flex flex-wrap items-baseline gap-2">
          {data.promotion ? (
            <>
              <span className="font-heading text-xl font-bold text-cc-heading">
                ${data.discountedPrice.toFixed(2)}
              </span>
              <span className="text-sm text-cc-ink-dim line-through">
                ${data.price.toFixed(2)}
              </span>
            </>
          ) : (
            <span className="font-heading text-xl font-bold text-cc-heading">
              ${data.price.toFixed(2)}
            </span>
          )}
        </div>
      </div>
    </article>
  );
}
