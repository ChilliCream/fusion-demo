import type { ComponentPropsWithoutRef } from "react";

/**
 * Shopping cart glyph, path data ported from the `#icon-cart` symbol in the
 * prototype (`prototype/store-look/index.html`).
 */
export function CartIcon(props: ComponentPropsWithoutRef<"svg">) {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" {...props}>
      <path
        d="M3 4h2.2l2.3 11.2a1.6 1.6 0 0 0 1.6 1.3h7.9a1.6 1.6 0 0 0 1.6-1.2L20.5 8H6.1"
        fill="none"
        stroke="currentColor"
        strokeWidth="1.7"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      <circle cx="9.8" cy="20" r="1.5" fill="currentColor" />
      <circle cx="16.6" cy="20" r="1.5" fill="currentColor" />
    </svg>
  );
}
