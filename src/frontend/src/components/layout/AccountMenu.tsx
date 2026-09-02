import { useEffect, useRef } from "react";
import { useAuth } from "../../auth/useAuth";

/**
 * Signed-in state of the header's account slot: a `<details>`-based menu
 * (no extra open/close state) showing the signed-in username with a Logout
 * action. Closes on an outside click, Escape, or picking the item - a
 * trimmed-down version of the website design system's
 * `Dropdown`/`DropdownAutoClose` pattern, since this store only ever needs
 * the one Logout action.
 */
export function AccountMenu() {
  const { user, logout } = useAuth();
  const detailsRef = useRef<HTMLDetailsElement>(null);

  useEffect(() => {
    const details = detailsRef.current;
    if (!details) {
      return;
    }

    const onPointerDown = (event: PointerEvent) => {
      if (details.open && !details.contains(event.target as Node)) {
        details.open = false;
      }
    };
    const onKeyDown = (event: KeyboardEvent) => {
      if (event.key === "Escape" && details.open) {
        details.open = false;
      }
    };

    document.addEventListener("pointerdown", onPointerDown);
    document.addEventListener("keydown", onKeyDown);
    return () => {
      document.removeEventListener("pointerdown", onPointerDown);
      document.removeEventListener("keydown", onKeyDown);
    };
  }, []);

  const handleLogout = () => {
    logout();
    if (detailsRef.current) {
      detailsRef.current.open = false;
    }
  };

  return (
    <details ref={detailsRef} className="relative">
      <summary className="flex cursor-pointer list-none items-center gap-1.5 rounded-full border border-cc-card-border px-4 py-2 text-[0.8125rem] font-medium text-cc-ink transition-colors select-none hover:border-cc-card-border-hover [&::-webkit-details-marker]:hidden">
        {user?.username}
      </summary>
      <div className="absolute top-full right-0 z-20 mt-2 w-40 overflow-hidden rounded-md border border-cc-card-border bg-cc-surface shadow-lg">
        <button
          type="button"
          onClick={handleLogout}
          className="block w-full cursor-pointer px-3 py-2 text-left text-sm text-cc-ink transition-colors hover:bg-cc-hover"
        >
          Logout
        </button>
      </div>
    </details>
  );
}
