import { OVERVIEW_GRID_CLASSES } from "./OverviewProductGrid";
import { ProductCardSkeleton } from "./ProductCardSkeleton";

interface ProductGridSkeletonProps {
  /** Number of skeleton tiles to render. Defaults to a full first page. */
  count?: number;
}

/** The route-level Suspense fallback: a grid of skeleton tiles. */
export function ProductGridSkeleton({ count = 12 }: ProductGridSkeletonProps) {
  return (
    <div className={OVERVIEW_GRID_CLASSES}>
      {Array.from({ length: count }, (_, index) => (
        <ProductCardSkeleton key={index} />
      ))}
    </div>
  );
}
