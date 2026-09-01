import { useEffect, useRef, useState } from "react";
import { fetchQuery, graphql, useRelayEnvironment } from "react-relay";
import type { Subscription } from "relay-runtime";
import type { DeliveryEstimateCardQuery } from "./__generated__/DeliveryEstimateCardQuery.graphql";
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
 * "Check" button. `Product.deliveryEstimate(zip)` is fired imperatively via
 * `fetchQuery` only when "Check" is clicked - never on mount, never while
 * typing - since it isn't part of the route's `useLazyLoadQuery` and
 * doesn't need to suspend the whole page. A GraphQL error (unrecognized
 * postal code) surfaces through the observable's `error` callback and
 * renders as the inline "No estimate for this postal code" line instead of
 * bubbling up to the route's error boundary. No validation beyond
 * non-empty - the "Check" button simply no-ops on a blank field.
 */
export function DeliveryEstimateCard({ productId }: DeliveryEstimateCardProps) {
  const environment = useRelayEnvironment();
  const [zip, setZip] = useState("");
  const [state, setState] = useState<EstimateState>({ kind: "idle" });
  const subscriptionRef = useRef<Subscription | null>(null);

  useEffect(() => {
    return () => {
      subscriptionRef.current?.unsubscribe();
    };
  }, []);

  function handleCheck() {
    const trimmed = zip.trim();
    if (!trimmed || state.kind === "loading") {
      return;
    }

    subscriptionRef.current?.unsubscribe();
    setState({ kind: "loading" });

    subscriptionRef.current = fetchQuery<DeliveryEstimateCardQuery>(
      environment,
      DeliveryEstimateCardQueryNode,
      { id: productId, zip: trimmed },
      { fetchPolicy: "network-only" },
    ).subscribe({
      next: (response) => {
        const days = response.productById?.deliveryEstimate;
        setState(
          typeof days === "number" ? { kind: "success", days } : { kind: "error" },
        );
      },
      error: () => {
        setState({ kind: "error" });
      },
    });
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
          onClick={handleCheck}
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
