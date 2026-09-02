import type { CSSProperties, ReactNode } from "react";

export interface CardProps {
  readonly children: ReactNode;
  readonly className?: string;
  /**
   * Root element. `"a"` renders a plain anchor and requires `href`; the
   * other values render the plain structural element.
   */
  readonly as?: "div" | "article" | "li" | "a";
  /** Destination for the whole card. Required when `as="a"`. */
  readonly href?: string;
  readonly target?: string;
  readonly rel?: string;
  /** Brightens the border on hover plus `transition-colors`. */
  readonly hoverBorder?: boolean;
  /** Decorative radial-gradient glow overlay in the top-right corner, clipped to the card. */
  readonly glow?: boolean;
  /** Inline styles for values Tailwind cannot express statically, such as a dynamic `gridRow` span. */
  readonly style?: CSSProperties;
}

/**
 * The bordered tile surface used across the store: a `cc-card-border`
 * outline over a blurred `cc-card-bg` fill, `rounded-xl`. Ported from
 * `Card.tsx` in the website design system (the `"tile"` variant), without
 * the Next.js `Link` dependency. Pass `hoverBorder` to brighten the border
 * on hover, and `glow` for the decorative cyan blob.
 */
export function Card({
  children,
  className,
  as = "div",
  href,
  target,
  rel,
  hoverBorder = false,
  glow = false,
  style,
}: CardProps) {
  const cls = [
    "relative overflow-hidden rounded-xl border border-cc-card-border bg-cc-card-bg p-6 backdrop-blur-sm",
    hoverBorder ? "transition-colors hover:border-cc-card-border-hover" : "",
    className ?? "",
  ]
    .filter(Boolean)
    .join(" ");

  const content = (
    <>
      {glow && <CardGlow />}
      {glow ? <span className="relative z-10">{children}</span> : children}
    </>
  );

  if (as === "a") {
    if (!href) {
      throw new Error('Card: `href` is required when `as="a"`.');
    }
    return (
      <a href={href} target={target} rel={rel} className={cls} style={style}>
        {content}
      </a>
    );
  }

  const Tag = as;
  return (
    <Tag className={cls} style={style}>
      {content}
    </Tag>
  );
}

function CardGlow() {
  return (
    <div
      aria-hidden="true"
      className="pointer-events-none absolute -top-24 right-0 -z-0 h-56 w-56 opacity-40 blur-3xl"
      style={{
        background:
          "radial-gradient(50% 50% at 60% 40%, rgba(22,185,228,0.18), transparent 70%)",
      }}
    />
  );
}
