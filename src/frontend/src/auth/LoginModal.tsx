import {
  useEffect,
  useRef,
  useState,
  type FormEvent,
  type MouseEvent as ReactMouseEvent,
  type PointerEvent as ReactPointerEvent,
} from "react";
import { Button } from "../components/ui/Button";
import { Input } from "../components/ui/Input";
import { useAuth } from "./useAuth";

/**
 * The signed-out "Sign in" flow: a centered dialog over a dimmed, blurred
 * backdrop, built from the ported Input/Button primitives (cc identity).
 * Demo credentials are prefilled since this store is wired to a fixed
 * Keycloak realm user. Mounted once (see `AppShell`) and shown/hidden via
 * `AuthContext`'s `isLoginModalOpen` state, so any page can trigger it
 * through `openLogin()` / `openLogin(pendingAdd)`.
 */
export function LoginModal() {
  const { isLoginModalOpen, closeLogin, login, loading, error } = useAuth();
  const [username, setUsername] = useState("demo-user");
  const [password, setPassword] = useState("demo-password");
  // Tracks whether the pointer actually went down on the backdrop itself
  // (rather than inside the dialog) so a text-selection drag that starts on
  // an input and is released over the backdrop doesn't close the modal: a
  // "click" event still fires on the backdrop in that case (browsers
  // resolve mousedown/mouseup target mismatches to the nearest common
  // ancestor), which would otherwise discard the pending add-to-cart intent.
  const pointerDownOnBackdrop = useRef(false);

  useEffect(() => {
    if (!isLoginModalOpen) {
      return;
    }

    const onKeyDown = (event: KeyboardEvent) => {
      if (event.key === "Escape") {
        closeLogin();
      }
    };
    document.addEventListener("keydown", onKeyDown);
    return () => document.removeEventListener("keydown", onKeyDown);
  }, [isLoginModalOpen, closeLogin]);

  if (!isLoginModalOpen) {
    return null;
  }

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();
    try {
      await login(username, password);
    } catch {
      // Failure is surfaced via `error` from AuthContext; modal stays open.
    }
  };

  function handleBackdropPointerDown(event: ReactPointerEvent<HTMLDivElement>) {
    pointerDownOnBackdrop.current = event.target === event.currentTarget;
  }

  function handleBackdropClick(event: ReactMouseEvent<HTMLDivElement>) {
    if (pointerDownOnBackdrop.current && event.target === event.currentTarget) {
      closeLogin();
    }
  }

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-cc-black/60 p-4 backdrop-blur-sm"
      onPointerDown={handleBackdropPointerDown}
      onClick={handleBackdropClick}
    >
      <div
        role="dialog"
        aria-modal="true"
        aria-labelledby="login-modal-title"
        className="w-full max-w-sm rounded-xl border border-cc-card-border bg-cc-card-bg p-6 shadow-lg backdrop-blur-[18px]"
        onClick={(event) => event.stopPropagation()}
      >
        <div className="mb-1 flex items-start justify-between gap-4">
          <h2
            id="login-modal-title"
            className="font-heading text-h6 font-semibold text-cc-heading"
          >
            Sign in
          </h2>
          <button
            type="button"
            aria-label="Close"
            onClick={closeLogin}
            className="cursor-pointer text-lg leading-none text-cc-ink-dim transition-colors hover:text-cc-heading"
          >
            &times;
          </button>
        </div>
        <p className="mb-5 text-sm text-cc-ink-dim">
          Demo credentials are prefilled below.
        </p>
        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <Input
            label="Username"
            name="username"
            autoComplete="username"
            value={username}
            onChange={(event) => setUsername(event.target.value)}
            autoFocus
            required
            disabled={loading}
          />
          <Input
            label="Password"
            name="password"
            type="password"
            autoComplete="current-password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            required
            disabled={loading}
          />
          {error && <p className="text-sm text-cc-danger">{error}</p>}
          <div className="mt-1 flex justify-end gap-3">
            <Button
              type="button"
              variant="outline"
              onClick={closeLogin}
              disabled={loading}
            >
              Cancel
            </Button>
            <Button type="submit" disabled={loading}>
              {loading ? "Signing in..." : "Sign in"}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}
