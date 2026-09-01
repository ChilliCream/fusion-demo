/** Base styling shared by text inputs (and future textareas). */
export const controlBaseClasses =
  "w-full rounded-md border bg-cc-white/5 px-3 py-2 text-sm focus:outline-hidden focus:ring-2 disabled:opacity-60";

/** Text, border, and focus classes that switch to an error state when invalid. */
export function controlBorderClasses(hasError: boolean) {
  return hasError
    ? "text-cc-danger placeholder:text-cc-danger/70 border-cc-danger focus:border-cc-danger focus:ring-cc-danger/30"
    : "text-cc-ink border-cc-card-border hover:border-cc-card-border-hover focus:border-cc-accent focus:ring-cc-accent/30";
}
