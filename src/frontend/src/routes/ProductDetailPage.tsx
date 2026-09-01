import { useParams } from "react-router";

/**
 * Placeholder for the product detail route (`/products/:id`). Filled in by
 * fusion-demo-js-0bx.6.
 */
export default function ProductDetailPage() {
  const { id } = useParams<{ id: string }>();

  return (
    <div className="mx-auto max-w-7xl px-6 py-16">
      <h1 className="font-heading text-h3 font-semibold text-cc-heading">
        Product {id}
      </h1>
      <p className="mt-3 text-cc-ink-dim">Product detail coming soon.</p>
    </div>
  );
}
