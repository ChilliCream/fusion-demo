/**
 * Route-level Suspense fallback for `/cart`: skeleton shapes for the
 * two-column line-items/summary layout, stacking at the same ~900px
 * breakpoint as the real layout (`CartView`), so nothing reflows once real
 * content replaces it.
 */
export function CartSkeleton() {
  return (
    <div aria-hidden="true" className="animate-pulse">
      <div className="mb-8 h-9 w-48 rounded bg-cc-hover" />

      <div className="grid grid-cols-[minmax(0,3fr)_minmax(0,2fr)] items-start gap-8 max-[900px]:grid-cols-1 max-[900px]:gap-6">
        <div className="flex flex-col gap-4">
          {Array.from({ length: 3 }, (_, index) => (
            <div
              key={index}
              className="flex items-center gap-4 rounded-xl border border-cc-card-border bg-cc-card-bg p-4"
            >
              <div className="h-20 w-20 flex-none rounded-lg bg-cc-hover" />
              <div className="flex flex-1 flex-col gap-2">
                <div className="h-4 w-1/2 rounded bg-cc-hover" />
                <div className="h-3 w-1/4 rounded bg-cc-hover" />
              </div>
              <div className="h-9 w-24 flex-none rounded-full bg-cc-hover" />
            </div>
          ))}
        </div>

        <div className="h-64 rounded-xl border border-cc-card-border bg-cc-card-bg" />
      </div>
    </div>
  );
}
