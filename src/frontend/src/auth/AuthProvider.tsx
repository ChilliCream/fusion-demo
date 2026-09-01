import { useEffect, useState, type ReactNode } from "react";
import { graphql, useMutation } from "react-relay";
import { AuthContext, type AuthUser, type PendingAdd } from "./AuthContext";
import type { AuthProviderAddToCartMutation } from "./__generated__/AuthProviderAddToCartMutation.graphql";
import { RelayEnvironment } from "../RelayEnvironment";

const KEYCLOAK_URL =
  import.meta.env.VITE_KEYCLOAK_URL || "http://localhost:8080";
const TOKEN_KEY = "auth_token";
const USER_KEY = "auth_user";

// Requests the CartBadge fragment (not just an id) so the header's cart
// count stays correct when a pending add-to-cart intent replays after
// login, without the auth module needing to know how the header renders it.
const AddToCartMutation = graphql`
  mutation AuthProviderAddToCartMutation($input: AddProductToCartInput!) {
    addProductToCart(input: $input) {
      cart {
        id
        ...CartBadge_cart
      }
    }
  }
`;

/**
 * Owns the Keycloak password-grant flow, exactly as it worked at
 * `src/AuthContext.tsx` (same endpoint, client id, grant type, and
 * `auth_token`/`auth_user` localStorage keys so `RelayEnvironment` keeps
 * working unchanged), plus the login modal's open/close state and the
 * pending "add to cart" intent that a signed-out add-to-cart click can
 * queue up via `openLogin(pendingAdd)`.
 */
export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(null);
  const [user, setUser] = useState<AuthUser | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isLoginModalOpen, setIsLoginModalOpen] = useState(false);
  const [pendingAdd, setPendingAdd] = useState<PendingAdd | null>(null);
  const [commitAddToCart] = useMutation<AuthProviderAddToCartMutation>(
    AddToCartMutation,
  );

  // Load token from localStorage on mount
  useEffect(() => {
    const storedToken = localStorage.getItem(TOKEN_KEY);
    const storedUser = localStorage.getItem(USER_KEY);
    if (storedToken && storedUser) {
      setToken(storedToken);
      setUser(JSON.parse(storedUser));
    }
  }, []);

  const login = async (username: string, password: string) => {
    setLoading(true);
    setError(null);

    try {
      const response = await fetch(
        `${KEYCLOAK_URL}/realms/fusion-demo/protocol/openid-connect/token`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/x-www-form-urlencoded",
          },
          body: new URLSearchParams({
            client_id: "frontend-app",
            username,
            password,
            grant_type: "password",
          }),
        },
      );

      if (!response.ok) {
        throw new Error("Invalid credentials");
      }

      const data = await response.json();
      const accessToken = data.access_token;

      // Store token and user info
      localStorage.setItem(TOKEN_KEY, accessToken);
      const userInfo = { username };
      localStorage.setItem(USER_KEY, JSON.stringify(userInfo));

      setToken(accessToken);
      setUser(userInfo);
      setIsLoginModalOpen(false);

      // Replay the pending add-to-cart intent, if any, exactly once. The
      // intent is cleared up front (not in a mutation callback) so it can
      // never fire twice, even if the mutation itself fails.
      if (pendingAdd) {
        const intent = pendingAdd;
        setPendingAdd(null);
        commitAddToCart({
          variables: {
            input: {
              productId: intent.productId,
              quantity: intent.quantity,
            },
          },
          onError: () => {
            // The cart write failing after a successful login must not
            // crash the app or block the now-signed-in state; the product
            // pages (later tasks) let the user retry the add manually.
          },
        });
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "Login failed");
      throw err;
    } finally {
      setLoading(false);
    }
  };

  const logout = () => {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    setToken(null);
    setUser(null);
    setError(null);

    // Mark every cached record stale so the next authenticated fetch (e.g.
    // a later sign-in, same user or not) re-hits the network instead of a
    // remounted component quietly resolving from this session's leftover
    // store data - otherwise the header's cart badge (and any other
    // authenticated view) can flash a stale count from before logout.
    // `notify(undefined, true)` is the store's public, typed entry point for
    // this (its `invalidateStore` boolean bumps the store's global
    // invalidation epoch); the dedicated `invalidateStore()` method exists
    // on the runtime's store implementation but isn't part of the `Store`
    // type this package ships.
    RelayEnvironment.getStore().notify(undefined, true);
  };

  const openLogin = (intent?: PendingAdd) => {
    setError(null);
    setPendingAdd(intent ?? null);
    setIsLoginModalOpen(true);
  };

  const closeLogin = () => {
    setIsLoginModalOpen(false);
    // Canceling the sign-in flow abandons whatever add-to-cart click
    // triggered it, so a later, unrelated login can't replay a stale intent.
    setPendingAdd(null);
  };

  return (
    <AuthContext.Provider
      value={{
        isAuthenticated: !!token,
        token,
        user,
        login,
        logout,
        loading,
        error,
        isLoginModalOpen,
        openLogin,
        closeLogin,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}
