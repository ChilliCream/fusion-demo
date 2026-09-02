import { ChilliCreamText } from "../icons/ChilliCreamText";

/**
 * Slim store footer: wordmark plus copyright line. Look ported from the
 * `.site-footer` rules in `prototype/store-look/index.html`.
 */
export function Footer() {
  const year = new Date().getFullYear();

  return (
    <footer className="border-t border-cc-card-border bg-cc-card-bg">
      <div className="mx-auto flex max-w-7xl flex-wrap items-center justify-between gap-4 px-6 py-4 text-[0.8125rem] text-cc-ink-dim">
        <ChilliCreamText className="h-4 w-auto text-cc-ink-dim" />
        <span>© {year} ChilliCream, Inc. · All Rights Reserved</span>
      </div>
    </footer>
  );
}
