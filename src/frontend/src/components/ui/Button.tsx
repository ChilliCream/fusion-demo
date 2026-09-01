import type { MouseEventHandler, ReactNode } from "react";

export type ButtonVariant = "solid" | "outline";

export interface ButtonProps {
  children: ReactNode;
  /** Destination for an anchor-style button. Omit to render a `<button>`. */
  href?: string;
  className?: string;
  /** Button `type` when rendered as a `<button>` (ignored for links). */
  type?: "button" | "submit";
  disabled?: boolean;
  /**
   * Styling preset. `"solid"` (default) is the filled cream pill used for
   * primary calls to action. `"outline"` is a hairline-border pill that
   * brightens on hover, used for secondary actions.
   */
  variant?: ButtonVariant;
  onClick?: MouseEventHandler<HTMLButtonElement | HTMLAnchorElement>;
  "aria-label"?: string;
}

const BASE_CLASSES =
  "inline-flex cursor-pointer items-center justify-center gap-2 rounded-full px-7 py-3 text-sm font-medium no-underline transition-colors disabled:cursor-not-allowed disabled:opacity-60";

// Filled pill: cream surface with the dark page color as the label.
const SOLID_CLASSES = "bg-cc-heading text-cc-surface hover:bg-cc-white";

// Outlined pill: hairline border that brightens on hover.
const OUTLINE_CLASSES =
  "border border-cc-card-border text-cc-ink hover:border-cc-card-border-hover";

const VARIANT_CLASSES: Record<ButtonVariant, string> = {
  solid: SOLID_CLASSES,
  outline: OUTLINE_CLASSES,
};

/**
 * The pill-shaped button used across the store: `Button.tsx` from the
 * ChilliCream website design system, ported without the Next.js `Link`
 * dependency (this app has no router yet). Renders a `<button>` by default,
 * or an `<a>` when `href` is supplied.
 */
export function Button({
  children,
  href,
  className,
  type = "button",
  disabled,
  variant = "solid",
  onClick,
  ...rest
}: ButtonProps) {
  const cls = [BASE_CLASSES, VARIANT_CLASSES[variant], className ?? ""]
    .filter(Boolean)
    .join(" ");

  if (href !== undefined) {
    return (
      <a href={href} className={cls} onClick={onClick} {...rest}>
        {children}
      </a>
    );
  }

  return (
    <button
      type={type}
      disabled={disabled}
      className={cls}
      onClick={onClick}
      {...rest}
    >
      {children}
    </button>
  );
}
