import { useEffect, useRef, useState } from "react";
import { graphql } from "react-relay";
import { getRequest } from "relay-runtime";
import type { DeliveryEstimateCardQuery } from "./__generated__/DeliveryEstimateCardQuery.graphql";
import { fetchFn } from "../../RelayEnvironment";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { Input } from "../../components/ui/Input";

const DeliveryEstimateCardQueryNode = graphql`
  query DeliveryEstimateCardQuery($id: ID!, $zip: String!) {
    productById(id: $id) {
      deliveryEstimate(zip: $zip)
    }
  }
`;

// `productById(id: $id)` is also selected (under the same name+args, so the
// same store slot) by the route's `ProductDetailPageQuery`. This lookup is
// fired imperatively and only needs a single `Int`, so it bypasses
// `fetchQuery`/the Relay store entirely and calls the network layer
// directly - an INVALID_POSTAL_CODE error null-propagating through the
// non-null `deliveryEstimate` field must never write `productById: null`
// into the shared slot and flip the whole product page to "not found".
const DeliveryEstimateCardRequest = getRequest(DeliveryEstimateCardQueryNode);

type EstimateResponseEnvelope = {
  data?: DeliveryEstimateCardQuery["response"] | null;
  errors?: ReadonlyArray<{ message: string }>;
};

function extractDays(response: EstimateResponseEnvelope): number | null {
  if (response.errors?.length) {
    return null;
  }
  const days = response.data?.productById?.deliveryEstimate;
  return typeof days === "number" ? days : null;
}

type EstimateState =
  | { kind: "idle" }
  | { kind: "loading" }
  | { kind: "success"; days: number }
  | { kind: "error" };

interface DeliveryEstimateCardProps {
  productId: string;
}

/**
 * The "Delivery estimate" tile: a postal code `Input` plus an outline
 * "Check" button. `Product.deliveryEstimate(zip)` is fired imperatively,
 * straight through the Relay network layer's `fetchFn` (see
 * `RelayEnvironment.ts`), only when "Check" is clicked - never on mount,
 * never while typing - since it isn't part of the route's
 * `useLazyLoadQuery` and doesn't need to suspend the whole page, and must
 * never touch the shared Relay store (see the comment on
 * `DeliveryEstimateCardRequest` above). A GraphQL error (unrecognized
 * postal code) or transport failure both render as the inline "No estimate
 * for this postal code" line instead of bubbling up to the route's error
 * boundary. No validation beyond non-empty - the "Check" button simply
 * no-ops on a blank field.
 */
export function DeliveryEstimateCard({ productId }: DeliveryEstimateCardProps) {
  const [zip, setZip] = useState("");
  const [state, setState] = useState<EstimateState>({ kind: "idle" });
  const requestIdRef = useRef(0);
  const isMountedRef = useRef(true);

  useEffect(() => {
    // Reset (rather than just declare via `useRef(true)`) so this is
    // correct across StrictMode's dev-only mount+unmount+remount cycle, not
    // just on the component's very first mount.
    isMountedRef.current = true;
    return () => {
      isMountedRef.current = false;
    };
  }, []);

  async function handleCheck() {
    const trimmed = zip.trim();
    if (!trimmed || state.kind === "loading") {
      return;
    }

    // Bumping the token both cancels the previous in-flight lookup (its
    // response is discarded as stale below) and gives this call "last wins"
    // priority over any future one.
    const requestId = ++requestIdRef.current;
    setState({ kind: "loading" });

    try {
      const variables: DeliveryEstimateCardQuery["variables"] = {
        id: productId,
        zip: trimmed,
      };
      const response = (await fetchFn(
        DeliveryEstimateCardRequest.params,
        variables,
        {},
      )) as EstimateResponseEnvelope;

      if (!isMountedRef.current || requestIdRef.current !== requestId) {
        return;
      }

      const days = extractDays(response);
      setState(days !== null ? { kind: "success", days } : { kind: "error" });
    } catch {
      if (isMountedRef.current && requestIdRef.current === requestId) {
        setState({ kind: "error" });
      }
    }
  }

  return (
    <Card className="mt-8">
      <h4 className="mb-1 font-heading text-h6 font-semibold text-cc-heading">
        Delivery estimate
      </h4>
      <p className="mb-4 text-[0.8125rem] text-cc-ink-dim">
        Enter a postal code to see delivery options.
      </p>
      <div className="mb-4 flex gap-3">
        <Input
          className="max-w-[12rem]"
          placeholder="Postal code"
          aria-label="Postal code"
          value={zip}
          onChange={(event) => setZip(event.target.value)}
        />
        <Button
          variant="outline"
          size="sm"
          disabled={state.kind === "loading"}
          onClick={() => {
            void handleCheck();
          }}
        >
          {state.kind === "loading" ? "Checking…" : "Check"}
        </Button>
      </div>

      {state.kind === "success" && (
        <div className="flex items-center gap-2 text-sm text-cc-ink">
          <span
            aria-hidden="true"
            className="h-2 w-2 flex-none rounded-full bg-cc-success"
          />
          Arrives in ~{state.days} days
        </div>
      )}

      {state.kind === "error" && (
        <p className="text-sm text-cc-danger">No estimate for this postal code.</p>
      )}
    </Card>
  );
}
