import { createContext } from "react";

/**
 * A single "add to cart" action captured when a signed-out visitor tries to
 * add a product. Carried by the login modal and replayed exactly once, right
 * after a successful sign-in, then discarded.
 */
export interface PendingAdd {
  productId: string;
  quantity: number;
}

export interface AuthUser {
  username: string;
}

export interface AuthContextValue {
  isAuthenticated: boolean;
  token: string | null;
  user: AuthUser | null;
  login: (username: string, password: string) => Promise<void>;
  logout: () => void;
  loading: boolean;
  error: string | null;
  /** Whether the login modal is currently open. */
  isLoginModalOpen: boolean;
  /**
   * Opens the login modal. Pass `pendingAdd` to have that add-to-cart
   * action replayed automatically, exactly once, right after a successful
   * login.
   */
  openLogin: (pendingAdd?: PendingAdd) => void;
  /** Closes the login modal and discards any pending add-to-cart intent. */
  closeLogin: () => void;
}

/**
 * Context object only - kept in its own file (no components) so it, and the
 * files that consume it, satisfy `react-refresh/only-export-components`.
 * See `AuthProvider.tsx` for the implementation and `useAuth.ts` for the
 * accessor hook.
 */
export const AuthContext = createContext<AuthContextValue | undefined>(
  undefined,
);
