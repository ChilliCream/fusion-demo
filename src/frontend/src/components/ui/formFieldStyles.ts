/** Base styling shared by text inputs (and future textareas). */
export const controlBaseClasses =
  "w-full rounded-md border bg-white/5 px-3 py-2 text-sm focus:outline-hidden focus:ring-2 disabled:opacity-60";

/** Text, border, and focus classes that switch to an error state when invalid. */
export function controlBorderClasses(hasError: boolean) {
  return hasError
    ? "text-red-500 placeholder:text-red-400 border-red-500 focus:border-red-500 focus:ring-red-500/30"
    : "text-cc-ink border-cc-card-border hover:border-cc-card-border-hover focus:border-cc-accent focus:ring-cc-accent/30";
}
