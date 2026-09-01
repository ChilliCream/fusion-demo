import { Link } from "react-router";
import { useAuth } from "../../auth/useAuth";
import { ChilliCreamWinking } from "../icons/ChilliCreamWinking";
import { ChilliCreamText } from "../icons/ChilliCreamText";
import { CartIcon } from "../icons/CartIcon";
import { Button } from "../ui/Button";
import { AccountMenu } from "./AccountMenu";
import { CartBadge } from "./CartBadge";

/**
 * Sticky store header: brand mark + "Store" eyebrow on the left, cart
 * shortcut and account slot on the right. Look ported from the
 * `.site-header` rules in `prototype/store-look/index.html`. Signed out,
 * the account slot opens the login modal (mounted by `AppShell`); signed
 * in, it shows the username with a Logout menu.
 */
export function Header() {
  const { isAuthenticated, openLogin } = useAuth();

  return (
    <header className="sticky top-0 z-40 h-18 border-b border-cc-card-border bg-cc-card-bg shadow-[inset_0_1px_0_rgba(255,255,255,0.06)] backdrop-blur-[18px]">
      <div className="mx-auto flex h-full max-w-7xl items-center justify-between gap-4 px-6">
        <Link
          to="/"
          className="flex items-center gap-2.5 text-cc-heading no-underline"
        >
          <ChilliCreamWinking className="h-8 w-8 flex-none" />
          <ChilliCreamText className="h-6 w-auto flex-none" />
          <span className="ml-1.5 border-l border-cc-card-border pt-[0.3em] pb-[0.1em] pl-3.5 font-heading text-[0.8125rem] leading-none font-semibold tracking-[0.24em] text-cc-accent uppercase">
            Store
          </span>
        </Link>
        <div className="flex items-center gap-3.5">
          <Link
            to="/cart"
            aria-label="Cart"
            className="relative inline-flex h-10 w-10 items-center justify-center rounded-full text-cc-heading transition-colors hover:bg-cc-hover"
          >
            <CartIcon className="h-[22px] w-[22px]" />
            {isAuthenticated && <CartBadge />}
          </Link>
          {isAuthenticated ? (
            <AccountMenu />
          ) : (
            <Button variant="outline" size="sm" onClick={() => openLogin()}>
              Sign in
            </Button>
          )}
        </div>
      </div>
    </header>
  );
}
