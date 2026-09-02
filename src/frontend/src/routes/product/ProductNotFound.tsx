import { Link } from "react-router";
import { Card } from "../../components/ui/Card";

/**
 * Demo-grade panel shown when `Query.productById` resolves to `null` - the
 * `:id` in the URL doesn't match any product. The "back" pill duplicates
 * `Button`'s outline/md classes rather than nesting a `<button>` inside a
 * `Link` (invalid markup) or using `Button`'s `href` (a plain `<a>`, which
 * would full-page-reload instead of a client-side route change).
 */
export function ProductNotFound() {
  return (
    <Card className="mx-auto max-w-md text-center">
      <p className="mb-5 text-cc-ink">This product could not be found.</p>
      <Link
        to="/"
        className="inline-flex items-center justify-center gap-2 rounded-full border border-cc-card-border px-7 py-3 text-sm font-medium text-cc-ink no-underline transition-colors hover:border-cc-card-border-hover"
      >
        Back to all products
      </Link>
    </Card>
  );
}
