export type StarRatingSize = "card" | "detail";

export interface StarRatingProps {
  /** Rating value, e.g. `4.5`. */
  rating: number;
  /** Total number of stars the rating is out of. Defaults to `5`. */
  maxRating?: number;
  /** `"card"` is the compact size used in dense product grids; `"detail"`
   * (default) is the larger size used on the product detail page and in
   * reviews. */
  size?: StarRatingSize;
  className?: string;
}

const SIZE_CLASSES: Record<StarRatingSize, string> = {
  card: "text-[0.78rem]",
  detail: "text-[0.9rem]",
};

/**
 * Star rating display: a fractional cyan/amber (`cc-warning`) fill
 * overlaid on a faint (`cc-ink-faint`) star row, clipped by width to the
 * rating's percentage. Ported from the `.stars` treatment in the
 * prototype (`prototype/store-look/index.html`).
 */
export function StarRating({
  rating,
  maxRating = 5,
  size = "detail",
  className,
}: StarRatingProps) {
  const pct =
    maxRating > 0 ? Math.max(0, Math.min(100, (rating / maxRating) * 100)) : 0;
  const stars = "★".repeat(Math.max(0, maxRating));

  return (
    <span
      role="img"
      aria-label={`Rated ${rating} out of ${maxRating}`}
      className={[
        "relative inline-block leading-none tracking-[0.14em] whitespace-nowrap text-cc-ink-faint",
        SIZE_CLASSES[size],
        className ?? "",
      ]
        .filter(Boolean)
        .join(" ")}
    >
      {stars}
      <span
        aria-hidden="true"
        className="pointer-events-none absolute top-0 left-0 overflow-hidden whitespace-nowrap text-cc-warning"
        style={{ width: `${pct}%` }}
      >
        {stars}
      </span>
    </span>
  );
}
