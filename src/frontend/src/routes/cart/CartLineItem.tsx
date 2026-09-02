import { useState } from "react";
import { graphql, useFragment, useMutation } from "react-relay";
import { Link } from "react-router";
import type { CartLineItem_cartItem$key } from "./__generated__/CartLineItem_cartItem.graphql";
import type { CartLineItemAddMutation } from "./__generated__/CartLineItemAddMutation.graphql";
import type { CartLineItemRemoveMutation } from "./__generated__/CartLineItemRemoveMutation.graphql";

const CartLineItemFragment = graphql`
  fragment CartLineItem_cartItem on CartItem {
    id
    quantity
    product {
      id
      name
      pictureUrl
      price
      discountedPrice
      promotion {
        id
      }
    }
  }
`;

// Both mutations re-spread CartBadge_cart (header badge) and CartView_cart
// (this page's own line items + summary rail) on their cart, same pattern as
// ProductDetailView/OverviewProductCard's adds, so every other line item and
// the summary stay in sync from a single edit here. Both select the
// payload's `errors` union - AddProductToCartError / RemoveProductFromCartError
// - and treat a non-empty list as failure (per fusion-demo-js-0bx.7 review
// comment #2): the overview/product-detail add-to-cart mutations predate
// that guidance and don't select it, but this page's mutations do.
const AddMutation = graphql`
  mutation CartLineItemAddMutation($input: AddProductToCartInput!) {
    addProductToCart(input: $input) {
      cart {
        id
        ...CartBadge_cart
        ...CartView_cart
      }
      errors {
        __typename
        ... on Error {
          message
        }
      }
    }
  }
`;

// Used for both the "−" stepper (quantity: 1) and the trash button
// (quantity: the item's full current quantity) - there's no set-quantity
// mutation, only this relative decrement, so both actions share one
// compiled mutation and only vary the `quantity` variable at the call site.
const RemoveMutation = graphql`
  mutation CartLineItemRemoveMutation($input: RemoveProductFromCartInput!) {
    removeProductFromCart(input: $input) {
      cart {
        id
        ...CartBadge_cart
        ...CartView_cart
      }
      errors {
        __typename
        ... on Error {
          message
        }
      }
    }
  }
`;

const GENERIC_ERROR_MESSAGE = "Something went wrong. Please try again.";

/** Simple "no picture" glyph, ported from the prototype's `#img-none`
 * symbol - duplicated locally rather than shared, same as
 * `ProductDetailView`'s and `OverviewProductCard`'s own copies (out of file
 * scope for this task). */
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

/** Trash glyph for the "remove line item" action. */
function TrashIcon({ className }: { className?: string }) {
  return (
    <svg
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth={1.7}
      strokeLinecap="round"
      strokeLinejoin="round"
      className={className}
      aria-hidden="true"
    >
      <path d="M4 7h16" />
      <path d="M9 7V5a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v2" />
      <path d="M6 7l1 13a2 2 0 0 0 2 2h6a2 2 0 0 0 2-2l1-13" />
      <path d="M10 11v6" />
      <path d="M14 11v6" />
    </svg>
  );
}

interface CartLineItemProps {
  item: CartLineItem_cartItem$key;
}

/**
 * A single row in the cart's line-items column: thumbnail (or placeholder),
 * name linking to `/products/:id`, unit price with promo treatment, a ±
 * stepper, the line total, and a trash button.
 *
 * There's no set-quantity mutation - "+" always calls
 * `addProductToCart(productId, 1)` and "−"/trash both call
 * `removeProductFromCart` (1, or the full current quantity for trash). "−"
 * is disabled at quantity 1 so the stepper itself never drives a line to
 * zero; trash is the only path there, and needs no confirmation (demo).
 *
 * All three actions share one `isBusy` flag so they can't race each other on
 * the same row. A mutation error - a non-empty `errors` entry on the
 * payload, a non-empty transport-level `errors` array (Relay's second
 * `onCompleted` argument, checked before the payload's own `errors`), or a
 * thrown GraphQL/network error via `onError` - is shown inline and leaves
 * the cart untouched (no optimistic update is applied; the row's displayed
 * `quantity` only changes once the mutation's re-spread `CartView_cart`
 * actually updates the underlying store record).
 */
export function CartLineItem({ item }: CartLineItemProps) {
  const data = useFragment(CartLineItemFragment, item);
  const [commitAdd] = useMutation<CartLineItemAddMutation>(AddMutation);
  const [commitRemove] =
    useMutation<CartLineItemRemoveMutation>(RemoveMutation);
  const [isBusy, setIsBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);

  function handleIncrement() {
    if (isBusy) {
      return;
    }
    setIsBusy(true);
    setError(null);
    commitAdd({
      variables: { input: { productId: data.product.id, quantity: 1 } },
      onCompleted: (response, transportErrors) => {
        setIsBusy(false);
        if (transportErrors?.length) {
          setError(GENERIC_ERROR_MESSAGE);
          return;
        }
        const payloadErrors = response.addProductToCart.errors ?? [];
        if (payloadErrors.length > 0) {
          setError(payloadErrors[0].message ?? GENERIC_ERROR_MESSAGE);
        }
      },
      onError: () => {
        setIsBusy(false);
        setError(GENERIC_ERROR_MESSAGE);
      },
    });
  }

  function handleDecrement() {
    if (isBusy || data.quantity <= 1) {
      return;
    }
    setIsBusy(true);
    setError(null);
    commitRemove({
      variables: { input: { productId: data.product.id, quantity: 1 } },
      onCompleted: (response, transportErrors) => {
        setIsBusy(false);
        if (transportErrors?.length) {
          setError(GENERIC_ERROR_MESSAGE);
          return;
        }
        const payloadErrors = response.removeProductFromCart.errors ?? [];
        if (payloadErrors.length > 0) {
          setError(payloadErrors[0].message ?? GENERIC_ERROR_MESSAGE);
        }
      },
      onError: () => {
        setIsBusy(false);
        setError(GENERIC_ERROR_MESSAGE);
      },
    });
  }

  function handleRemove() {
    if (isBusy) {
      return;
    }
    setIsBusy(true);
    setError(null);
    commitRemove({
      variables: {
        input: { productId: data.product.id, quantity: data.quantity },
      },
      onCompleted: (response, transportErrors) => {
        setIsBusy(false);
        if (transportErrors?.length) {
          setError(GENERIC_ERROR_MESSAGE);
          return;
        }
        const payloadErrors = response.removeProductFromCart.errors ?? [];
        if (payloadErrors.length > 0) {
          setError(payloadErrors[0].message ?? GENERIC_ERROR_MESSAGE);
        }
      },
      onError: () => {
        setIsBusy(false);
        setError(GENERIC_ERROR_MESSAGE);
      },
    });
  }

  const unitPrice = data.product.promotion
    ? data.product.discountedPrice
    : data.product.price;
  const lineTotal = unitPrice * data.quantity;

  return (
    <div className="flex flex-col gap-4 rounded-xl border border-cc-card-border bg-cc-card-bg p-4 backdrop-blur-sm sm:flex-row sm:items-center">
      <Link
        to={`/products/${data.product.id}`}
        className="h-20 w-20 flex-none overflow-hidden rounded-lg border border-cc-card-border"
      >
        {data.product.pictureUrl ? (
          <img
            src={data.product.pictureUrl}
            alt={data.product.name}
            className="h-full w-full object-cover"
          />
        ) : (
          <div className="flex h-full w-full items-center justify-center bg-cc-hover text-cc-ink-faint">
            <NoImageIcon className="h-8 w-8" />
          </div>
        )}
      </Link>

      <div className="min-w-0 flex-1">
        <Link
          to={`/products/${data.product.id}`}
          className="font-heading text-base font-semibold text-cc-heading no-underline hover:text-cc-accent"
        >
          {data.product.name}
        </Link>
        <div className="mt-1 flex flex-wrap items-baseline gap-2 text-sm">
          {data.product.promotion ? (
            <>
              <span className="font-semibold text-cc-heading">
                ${data.product.discountedPrice.toFixed(2)}
              </span>
              <span className="text-cc-ink-dim line-through">
                ${data.product.price.toFixed(2)}
              </span>
            </>
          ) : (
            <span className="font-semibold text-cc-heading">
              ${data.product.price.toFixed(2)}
            </span>
          )}
        </div>
        {error && (
          <p role="alert" className="mt-2 text-sm text-cc-danger">
            {error}
          </p>
        )}
      </div>

      <div className="flex items-center justify-between gap-4 sm:flex-col sm:items-end sm:justify-start">
        <div
          className="inline-flex items-center rounded-full border border-cc-card-border"
          aria-label={`Quantity for ${data.product.name}`}
        >
          <button
            type="button"
            aria-label="Decrease quantity"
            disabled={isBusy || data.quantity <= 1}
            className="flex h-9 w-9 items-center justify-center rounded-full text-cc-heading transition-colors hover:bg-cc-hover disabled:cursor-not-allowed disabled:opacity-40"
            onClick={handleDecrement}
          >
            −
          </button>
          <output
            aria-live="polite"
            className="min-w-[2rem] text-center text-sm font-semibold text-cc-heading"
          >
            {data.quantity}
          </output>
          <button
            type="button"
            aria-label="Increase quantity"
            disabled={isBusy}
            className="flex h-9 w-9 items-center justify-center rounded-full text-cc-heading transition-colors hover:bg-cc-hover disabled:cursor-not-allowed disabled:opacity-40"
            onClick={handleIncrement}
          >
            +
          </button>
        </div>

        <div className="flex items-center gap-3">
          <span className="font-heading text-sm font-semibold text-cc-heading">
            ${lineTotal.toFixed(2)}
          </span>
          <button
            type="button"
            aria-label={`Remove ${data.product.name} from cart`}
            disabled={isBusy}
            className="text-cc-ink-dim transition-colors hover:text-cc-danger disabled:cursor-not-allowed disabled:opacity-40"
            onClick={handleRemove}
          >
            <TrashIcon className="h-5 w-5" />
          </button>
        </div>
      </div>
    </div>
  );
}
