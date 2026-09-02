import type { ReactNode } from "react";

interface CommonProps {
  children: ReactNode;
  className?: string;
  /** Native browser tooltip shown on hover. */
  title?: string;
}

interface LinkTagProps extends CommonProps {
  href: string;
}

interface StaticTagProps extends CommonProps {
  href?: undefined;
}

export type TagProps = LinkTagProps | StaticTagProps;

const BASE_CLASSES =
  "inline-flex items-center rounded-full border border-cc-card-border bg-cc-hover px-3 py-1 text-xs font-medium text-cc-ink-dim no-underline transition-colors";
const INTERACTIVE_CLASSES =
  "hover:border-cc-accent-hover hover:bg-cc-accent/10 hover:text-cc-accent-hover";

/**
 * The hairline pill used for category/status labels, ported from `Tag.tsx`
 * in the website design system without the Next.js `Link` dependency.
 * Renders a `<span>`, or an `<a>` when `href` is supplied.
 */
export function Tag(props: TagProps) {
  const { children, className, title } = props;
  const cls = [
    BASE_CLASSES,
    props.href ? INTERACTIVE_CLASSES : "",
    className ?? "",
  ]
    .filter(Boolean)
    .join(" ");

  if (props.href) {
    return (
      <a href={props.href} className={cls} title={title}>
        {children}
      </a>
    );
  }
  return (
    <span className={cls} title={title}>
      {children}
    </span>
  );
}
