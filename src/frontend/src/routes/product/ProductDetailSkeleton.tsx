/**
 * Route-level Suspense fallback for `/products/:id`: skeleton shapes for the
 * breadcrumb, the 60/40 media/rail grid (stacking at the same ~900px
 * breakpoint as the real layout), and the reviews section below - so
 * nothing reflows once real content replaces it.
 */
export function ProductDetailSkeleton() {
  return (
    <div aria-hidden="true" className="animate-pulse">
      <div className="mb-8 h-4 w-40 rounded bg-cc-hover" />

      <div className="grid grid-cols-[minmax(0,3fr)_minmax(0,2fr)] items-start gap-11 max-[900px]:grid-cols-1 max-[900px]:gap-7">
        <div className="aspect-[4/3] rounded-2xl border border-cc-card-border bg-cc-card-bg" />

        <div className="flex flex-col gap-4">
          <div className="h-9 w-3/4 rounded bg-cc-hover" />
          <div className="h-4 w-1/2 rounded bg-cc-hover" />
          <div className="h-8 w-1/3 rounded bg-cc-hover" />
          <div className="h-11 w-48 rounded-full bg-cc-hover" />
          <div className="mt-4 h-36 rounded-xl border border-cc-card-border bg-cc-card-bg" />
        </div>
      </div>

      <div className="mt-16">
        <div className="mb-6 h-7 w-32 rounded bg-cc-hover" />
        <div className="grid grid-cols-[repeat(auto-fit,minmax(260px,1fr))] gap-5">
          {Array.from({ length: 3 }, (_, index) => (
            <div
              key={index}
              className="h-36 rounded-xl border border-cc-card-border bg-cc-card-bg"
            />
          ))}
        </div>
      </div>
    </div>
  );
}
