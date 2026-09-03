import { useState, type ChangeEvent, type FormEvent } from "react";
import { graphql, useFragment, useMutation } from "react-relay";
import type { CartPromoCode_cart$key } from "./__generated__/CartPromoCode_cart.graphql";
import type { CartPromoCodeApplyMutation } from "./__generated__/CartPromoCodeApplyMutation.graphql";
import type { CartPromoCodeRemoveMutation } from "./__generated__/CartPromoCodeRemoveMutation.graphql";
import { Button } from "../../components/ui/Button";
import { Input } from "../../components/ui/Input";

const CartPromoCodeFragment = graphql`
  fragment CartPromoCode_cart on Cart {
    id
    promoCode {
      code
      title
      discountPercent
      isExpired
    }
  }
`;

// Both mutations re-spread CartBadge_cart (header badge) and CartView_cart
// (this page's line items + summary rail, which re-selects `promoCode`) on
// their cart, same pattern as CartLineItem's add/remove mutations - the
// design spec confirmed the cross-schema `promoCode`/money fields resolve
// under a mutation payload on this stack (fusion-demo-yt-sry.12 addendum),
// so no separate refetch is needed. Both select the payload's `errors`
// union and treat a non-empty list as failure, never a thrown error.
const ApplyPromoCodeMutation = graphql`
  mutation CartPromoCodeApplyMutation($input: ApplyPromoCodeInput!) {
    applyPromoCode(input: $input) {
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

const RemovePromoCodeMutation = graphql`
  mutation CartPromoCodeRemoveMutation($input: RemovePromoCodeInput!) {
    removePromoCode(input: $input) {
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

/** Maps `ApplyPromoCodeError`'s two member types to the copy the design
 * spec calls for; any other `__typename` (there shouldn't be one, but the
 * union could grow) falls back to the server's own message. */
function applyErrorMessage(error: { __typename: string; message?: string }) {
  switch (error.__typename) {
    case "PromoCodeNotFoundError":
      return "We don't know that code";
    case "PromoCodeExpiredError":
      return "That code has expired";
    default:
      return error.message || GENERIC_ERROR_MESSAGE;
  }
}

interface CartPromoCodeProps {
  cart: CartPromoCode_cart$key;
}

/**
 * The cart summary's promo code row: an empty-state text input + Apply
 * button, an applied-state chip with a remove action, or the input again
 * with an inline rejection message. `applyPromoCode`/`removePromoCode` take
 * `Cart.id` (this fragment's own `id`, the same global id `CartView_cart`
 * carries) - never a thrown error, only the payload's `errors` union.
 *
 * There's no optimistic update: the applied/empty state shown is whatever
 * `CartView_cart`'s re-spread `promoCode` currently holds in the Relay
 * store, same as `CartLineItem`'s quantity.
 */
export function CartPromoCode({ cart }: CartPromoCodeProps) {
  const data = useFragment(CartPromoCodeFragment, cart);
  const [commitApply] =
    useMutation<CartPromoCodeApplyMutation>(ApplyPromoCodeMutation);
  const [commitRemove] =
    useMutation<CartPromoCodeRemoveMutation>(RemovePromoCodeMutation);
  const [code, setCode] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  function handleCodeChange(event: ChangeEvent<HTMLInputElement>) {
    setCode(event.target.value.toUpperCase());
    if (error) {
      setError(null);
    }
  }

  function handleApply(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const trimmed = code.trim();
    if (isSubmitting || trimmed.length === 0) {
      return;
    }
    setIsSubmitting(true);
    setError(null);
    commitApply({
      variables: { input: { cartId: data.id, code: trimmed } },
      onCompleted: (response, transportErrors) => {
        setIsSubmitting(false);
        if (transportErrors?.length) {
          setError(GENERIC_ERROR_MESSAGE);
          return;
        }
        const payloadErrors = response.applyPromoCode.errors ?? [];
        if (payloadErrors.length > 0) {
          setError(applyErrorMessage(payloadErrors[0]));
          return;
        }
        setCode("");
      },
      onError: () => {
        setIsSubmitting(false);
        setError(GENERIC_ERROR_MESSAGE);
      },
    });
  }

  function handleRemove() {
    if (isSubmitting) {
      return;
    }
    setIsSubmitting(true);
    setError(null);
    commitRemove({
      variables: { input: { cartId: data.id } },
      onCompleted: (response, transportErrors) => {
        setIsSubmitting(false);
        if (transportErrors?.length) {
          setError(GENERIC_ERROR_MESSAGE);
          return;
        }
        const payloadErrors = response.removePromoCode.errors ?? [];
        if (payloadErrors.length > 0) {
          setError(payloadErrors[0].message ?? GENERIC_ERROR_MESSAGE);
        }
      },
      onError: () => {
        setIsSubmitting(false);
        setError(GENERIC_ERROR_MESSAGE);
      },
    });
  }

  if (data.promoCode) {
    const { code: appliedCode, title } = data.promoCode;
    return (
      <div className="flex flex-col gap-2">
        <span className="text-sm font-medium text-cc-ink">Promo code</span>
        <div className="flex items-center justify-between gap-2 rounded-full border border-cc-success/35 bg-cc-success/10 py-1.5 pl-4 pr-1.5">
          <span className="truncate text-sm font-medium text-cc-success">
            {appliedCode} · {title}
          </span>
          <button
            type="button"
            aria-label="Remove promo code"
            disabled={isSubmitting}
            onClick={handleRemove}
            className="flex h-6 w-6 flex-none items-center justify-center rounded-full text-cc-success transition-colors hover:bg-cc-success/20 disabled:cursor-not-allowed disabled:opacity-40"
          >
            ×
          </button>
        </div>
        {error && (
          <p role="alert" className="text-sm text-cc-danger">
            {error}
          </p>
        )}
      </div>
    );
  }

  return (
    <form onSubmit={handleApply} className="flex flex-col gap-2">
      <div className="flex items-end gap-2">
        <div className="flex-1">
          <Input
            label="Promo code"
            placeholder="Promo code"
            value={code}
            onChange={handleCodeChange}
            disabled={isSubmitting}
            error={error ?? undefined}
          />
        </div>
        <Button
          type="submit"
          variant="outline"
          size="md"
          disabled={isSubmitting || code.trim().length === 0}
        >
          Apply
        </Button>
      </div>
    </form>
  );
}
