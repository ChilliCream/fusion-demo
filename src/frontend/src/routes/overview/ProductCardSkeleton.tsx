/**
 * Loading placeholder for a single grid tile, shaped like `ProductCard`
 * (16:10 media block, name and price bars) but with no data - shown only
 * while the route-level Suspense boundary is pending the first page.
 */
export function ProductCardSkeleton() {
  return (
    <div
      aria-hidden="true"
      className="flex h-full animate-pulse flex-col overflow-hidden rounded-xl border border-cc-card-border bg-cc-card-bg backdrop-blur-sm"
    >
      <div className="aspect-[16/10] w-full border-b border-cc-card-border bg-cc-hover" />
      <div className="flex flex-1 flex-col gap-3 px-5 pt-[1.1rem] pb-[1.3rem]">
        <div className="h-4 w-3/4 rounded bg-cc-hover" />
        <div className="h-3 w-1/2 rounded bg-cc-hover" />
        <div className="mt-auto h-5 w-1/3 rounded bg-cc-hover" />
      </div>
    </div>
  );
}
