# Pagination

Pagination is cursor-based, built on the `Page<T>`/`PagingArguments` primitives and surfaced as GraphQL connections via `[UseConnection]`. Two placements, two patterns: root fields page straight off the queryable; fields on entity types page through a batch-paging DataLoader. Whether a field should be paginated at all is a schema-design question (see the `graphql-design` skill) — here is how to implement it.

## Never offset pagination

Do not implement offset (skip/take) pagination. Offsets drift as rows are inserted or deleted — pages skip or repeat — and the database must scan past every skipped row. When the frontend needs offset-style concepts — jump to page X, numbered page links — use **relative cursors** instead: page-jump navigation on top of stable cursor pagination, with the data access staying cursor-based.

Enable them per field, or centrally for the whole schema:

```csharp
[UseConnection(EnableRelativeCursors = true)]
```

```csharp
builder
    .AddGraphQL()
    .ModifyPagingOptions(o => o.EnableRelativeCursors = true);
```

If offset-style navigation is not required, leave relative cursors disabled — they are an opt-in capability for frontends that need page jumps, not a default.

## Paging options

Set the page-size bounds explicitly — never ride on framework defaults:

```csharp
builder
    .AddGraphQL()
    .ModifyPagingOptions(o =>
    {
        o.DefaultPageSize = 25;
        o.MaxPageSize = 150;
        o.IncludeTotalCount = true;
    });
```

- `MaxPageSize = 150` and `DefaultPageSize = 25` are good baseline values; tune them per schema, but always set them deliberately.
- When `totalCount` is needed on pages, either opt in per field with `[UseConnection(IncludeTotalCount = true)]` or enable it globally as shown. Either way, the count query only runs when the client actually selects `totalCount`.
- The null-ordering setting belongs in the same options block (see the ordering rules below).

## Cursor pagination needs an order

A cursor encodes a position in an ordered sequence. Without a deterministic order the sequence shifts between pages — rows repeat or vanish. Two rules:

1. **Every paginated query states an order.** Either a plain `OrderBy` on the queryable, or — when the field composes with `UseSorting` (see [sorting.md](sorting.md) for what to allow clients to sort on) — a *default order* passed alongside the query context so a client-supplied sort wins and the default applies otherwise. This is enforced: `ToPageAsync`/`ToBatchPageAsync` throw an `ArgumentException` when the queryable has no ordering key.
2. **The order ends in the key.** The last column(s) of the order must be the entity's key, making the total order distinct — otherwise a cursor can yield back the wrong rows, because non-unique sort columns (score, name, date) leave ties whose relative order the database may change between queries. With a single-column key, append `Id` last; with a composite key, append every key column. This holds no matter who supplied the sort.
3. **Specify the null ordering.** Databases disagree on where `null` sorts — set it explicitly on the paging options so the behavior is defined independent of the database:

   ```csharp
   builder
       .AddGraphQL()
       .ModifyPagingOptions(o => o.NullOrdering = NullOrdering.NativeNullsLast);
   ```

The default order is a `SortDefinition<T>` transform:

```csharp
private static SortDefinition<Recommendation> DefaultOrder(
    SortDefinition<Recommendation> sort)
    => sort
        .IfEmpty(o => o.AddDescending(recommendation => recommendation.Score))
        .AddAscending(recommendation => recommendation.Id);
```

`IfEmpty` applies its ordering only when the consumer (e.g. a `UseSorting` client) did not provide one; the `AddAscending(x => x.Id)` after it is appended *always*, so every order — default or client-supplied — ends in the key.

## Root fields: page the queryable directly

When the field has no client-controlled sorting (no `UseSorting`), state the order inline — this is the simplest form to grasp, and the one to prefer:

```csharp
[QueryType]
internal static partial class RecommendationQueries
{
    [UseConnection]
    public static async Task<PageConnection<Recommendation>> GetRecommendationsAsync(
        PagingArguments pagingArguments,
        QueryContext<Recommendation> query,
        RecommendationContext context,
        CancellationToken cancellationToken)
        => await context.Recommendations
            .AsNoTracking()
            .OrderBy(t => t.CreatedAt)
            .ThenBy(t => t.Id)
            .With(query)
            .ToPageAsync(pagingArguments, cancellationToken);
}
```

When the field composes with `UseSorting`, an inline `OrderBy` would be overwritten by the client's sort — pass a default order alongside the query context instead, so a client-supplied sort wins and the fallback applies otherwise:

```csharp
        => await context.Recommendations
            .AsNoTracking()
            .With(query, DefaultOrder)
            .ToPageAsync(pagingArguments, cancellationToken);
```

- `[UseConnection]` exposes the field with connection arguments (`first`/`after`/`last`/`before`) and the connection result shape. Configure paging behavior on the attribute when needed: `DefaultPageSize`, `MaxPageSize`, `IncludeTotalCount`, `RequirePagingBoundaries`, and `Name` to override the connection type name.
- `PagingArguments` receives those arguments; pass it through to `ToPageAsync`. Parameter injection requires `.AddPagingArguments()` on the server builder (see setup below). `totalCount` is only computed when the client actually selects it.
- `QueryContext<Recommendation>` carries the client's selection, filter, and sort into the database query. It must be typed to the connection's node type (`QueryContext<Recommendation>` on a `Recommendation` connection — the HC0101 analyzer enforces this).
- `.With(query)` applies the context as filter → sort → projection. Without `UseSorting` the context carries no sort, so the inline `OrderBy`/`ThenBy` stands; with `UseSorting`, pass `DefaultOrder` as the second argument to guarantee an order when the client supplies none.
- The resolver returns the `Page<T>` directly — `PageConnection<T>` has an implicit conversion from `Page<T>`.

Server setup for this stack:

```csharp
builder
    .AddGraphQL()
    .AddPagingArguments()
    .AddQueryContext()
    .AddSorting()
    .AddFiltering();
```

## Fields on entity types: page through a DataLoader

A paging field on any non-root type must use a DataLoader — the same N+1 logic as every other relation, made worse because each parent needs its own page slice. `ToBatchPageAsync` batches the slicing of *all* parents' pages into a single database request:

```csharp
internal static class RecommendationDataLoader
{
    [DataLoader]
    public static async Task<Dictionary<int, Page<Recommendation>>> GetRecommendationsByUserIdAsync(
        IReadOnlyList<int> userIds,
        PagingArguments pagingArguments,
        QueryContext<Recommendation> query,
        RecommendationContext context,
        CancellationToken cancellationToken)
        => await context.Recommendations
            .AsNoTracking()
            .Where(recommendation => userIds.Contains(recommendation.UserId))
            .With(query.Include(t => t.UserId), DefaultOrder)
            .ToBatchPageAsync(
                recommendation => recommendation.UserId,
                pagingArguments,
                cancellationToken);

    private static SortDefinition<Recommendation> DefaultOrder(
        SortDefinition<Recommendation> sort)
        => sort
            .IfEmpty(o => o.AddDescending(recommendation => recommendation.Score))
            .AddAscending(recommendation => recommendation.Id);
}
```

The rules from [dataloaders.md](dataloaders.md) all still apply, plus the paging-specific ones:

- Return `Dictionary<TKey, Page<TValue>>` — one page per parent key.
- Take `PagingArguments` as a parameter; the consumer's paging state reaches the fetch through it.
- State the **partition key** in the projection (`query.Include(t => t.UserId)`), keeping the key-pinning rule from [dataloaders.md](dataloaders.md) uniform. On the `ToBatchPageAsync` path the key-selector property is auto-included, so this is belt-and-braces here — but `ToDictionaryAsync` loaders have no such safety net, so pin the key everywhere.
- Order the same way as at the root: inline `OrderBy`/`ThenBy` before `.With(...)` when the field has no client sorting, or the `DefaultOrder` transform when it composes with `UseSorting`.

The consuming resolver on the entity type:

```csharp
[ObjectType<User>]
public static partial class UserType
{
    [UseConnection]
    public static async Task<PageConnection<Recommendation>> GetRecommendationsAsync(
        [Parent] User user,
        PagingArguments pagingArguments,
        QueryContext<Recommendation> query,
        RecommendationsByUserIdDataLoader recommendationsByUserId,
        CancellationToken cancellationToken)
        => await recommendationsByUserId
            .With(pagingArguments, query)
            .LoadAsync(user.Id, cancellationToken)
            ?? Page<Recommendation>.Empty;
}
```

`.With(pagingArguments, query)` branches the DataLoader per paging state + query context, so all parents requesting the same page shape share one batch. `LoadAsync` returns `null` for a parent with no rows — coalesce to `Page<T>.Empty`, which converts to an empty connection.

## Wrong-vs-right

**Wrong — paging a relation without a DataLoader** (one paged query per parent):

```csharp
[UseConnection]
public static async Task<PageConnection<Recommendation>> GetRecommendationsAsync(
    [Parent] User user,
    PagingArguments pagingArguments,
    QueryContext<Recommendation> query,
    RecommendationContext context,          // N+1: executes per User
    CancellationToken cancellationToken)
    => await context.Recommendations
        .Where(r => r.UserId == user.Id)
        .With(query, DefaultOrder)
        .ToPageAsync(pagingArguments, cancellationToken);
```

**Wrong — order without the key:**

```csharp
=> await context.Recommendations
    .AsNoTracking()
    .OrderBy(t => t.Score)                  // non-distinct: rows tie on Score
    .With(query)
    .ToPageAsync(pagingArguments, cancellationToken);
```

Rows that tie on `Score` have no defined relative order, so a cursor into a tie can yield wrong rows on the next page. This is the *silent* failure — forgetting the order entirely is caught loudly (`ToPageAsync` throws an `ArgumentException`), but a non-distinct order pages happily until a tie bites.

**Wrong — materializing to page in memory:**

```csharp
=> (await context.Recommendations.ToListAsync(cancellationToken))   // loads the table
    .Where(r => r.UserId == user.Id)
    ...
```

Paging exists to bound the query; `ToPageAsync`/`ToBatchPageAsync` push slicing into SQL.

## Gotchas

- A non-distinct order is the classic silent failure: everything works until two rows tie on the sort column and a client walks pages across the tie — the cursor then yields wrong rows. Always end the order with the key: `Id` for a single-column key, every key column for a composite key.
- Bare list fields that can grow unbounded should not exist — that is a schema-design rule, but implementation reviews should flag any unpaginated collection resolver backed by an unbounded query.
