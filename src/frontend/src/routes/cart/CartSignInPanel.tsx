import { useAuth } from "../../auth/useAuth";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";

/**
 * Shown at `/cart` when the visitor is signed out. `Query.viewer.cart` is an
 * authenticated field, so `CartPage` never even runs the cart query in this
 * state (see `CartPage`'s `isAuthenticated` gate) - this panel opens the
 * shared login modal instead. Once signed in, `CartPage` swaps this panel
 * for the live cart query in place, no separate replay wiring needed here.
 */
export function CartSignInPanel() {
  const { openLogin } = useAuth();

  return (
    <Card className="mx-auto max-w-md text-center">
      <p className="mb-5 text-cc-ink">Sign in to see your cart</p>
      <Button variant="solid" size="md" onClick={() => openLogin()}>
        Sign in
      </Button>
    </Card>
  );
}
