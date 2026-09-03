# DataLoaders

DataLoaders are the data-access primitive of a HotChocolate server. They batch all loads for a key set into one database roundtrip and cache results for the duration of the request, which removes N+1 loads and guarantees data consistency within a single execution. In this codebase style, **every** fetch — relations *and* top-level lookups — flows through a DataLoader.

## Source-generate, never hand-write

Write a static method annotated with `[DataLoader]` inside an internal static class. The HotChocolate source generator (`HotChocolate.Types.Analyzers` package) emits the DataLoader class, its interface, and the DI registration. Never hand-write a class deriving from `BatchDataLoader`/`GroupedDataLoader`, and never wire DataLoaders into DI yourself — registration belongs to the generated module (see below).

The canonical batch DataLoader:

```csharp
internal static class RecommendationDataLoader
{
    [DataLoader]
    public static async Task<Dictionary<int, Recommendation>> GetRecommendationByIdAsync(
        IReadOnlyList<int> ids,
        QueryContext<Recommendation> query,
        RecommendationContext context,
        CancellationToken cancellationToken)
        => await context.Recommendations
            .AsNoTracking()
            .Where(recommendation => ids.Contains(recommendation.Id))
            .With(query.Include(recommendation => recommendation.Id))
            .ToDictionaryAsync(
                recommendation => recommendation.Id,
                cancellationToken);
}
```

Every piece of this shape is a rule:

- **`IReadOnlyList<int> ids` first.** The first parameter is always the key list. It is a rented list — never store it or use it after the method returns; copy the keys if you need them longer.
- **`QueryContext<TValue> query` always.** The query context carries the consumer's projection (selector), filter (predicate), and sorting down to the database, so a DataLoader only fetches the columns the GraphQL selection actually needs. Declare it on every DataLoader — the generator passes an empty context when no consumer supplies one, so it is never null.
- **Always pin the key into the projection: `.With(query.Include(x => x.Id))`.** When a consumer passes a narrowed projection, the key column would otherwise not be selected — and then the result dictionary cannot be keyed. State the key property explicitly on every DataLoader via `query.Include`.
- **`AsNoTracking()`.** DataLoader reads are read-only; skip EF Core change tracking.
- **`ids.Contains(...)`** translates to a single SQL `IN` — one roundtrip for the whole batch.
- **Services as parameters.** The `DbContext` (and any other service) is a plain method parameter resolved from DI per fetch. Do not resolve services inside the method body.

### Generated names

The generator strips the `Get` prefix and `Async` suffix from the method name and appends `DataLoader`:

| Fetch method | Generated class | Generated interface |
| --- | --- | --- |
| `GetRecommendationByIdAsync` | `RecommendationByIdDataLoader` | `IRecommendationByIdDataLoader` |
| `GetRecommendationsByUserIdAsync` | `RecommendationsByUserIdDataLoader` | `IRecommendationsByUserIdDataLoader` |

Prefer injecting the generated **class** (`RecommendationByIdDataLoader`) over the interface — go-to-definition on the class jumps straight to your fetch code, while the interface lands on generated code. The interface exists for consumers that need an abstraction seam.

### Registration

Nothing per-loader. The project's explicit module (see the project setup section in [SKILL.md](../SKILL.md)) covers DataLoaders too — `[assembly: Module("RecommendationTypes")]` makes the generator emit `AddRecommendationTypes()`, which registers every generated type *and* DataLoader of the project:

```csharp
builder
    .AddGraphQL()
    .AddRecommendationTypes();
```

Declaring the generated DataLoader as a resolver parameter is all a consumer needs — the request-scoped instance is injected.

Only a HotChocolate-free data-layer assembly (GreenDonut without GraphQL types) uses `[assembly: DataLoaderModule("RecommendationDataLoaders")]` instead, which emits an `IServiceCollection` extension: `builder.Services.AddRecommendationDataLoaders()`.

### Attribute options

- `[DataLoader("BrandLookup")]` overrides the generated name (positional argument only — `Name = "..."` does not compile); the value gets `DataLoader` appended.
- `MaxBatchSize` (16.4+) caps the keys handed to one fetch call. Default 1024; an explicit `0` disables splitting. Split batches are dispatched concurrently.
- `Lookups` names methods on the same class that derive additional cache keys from loaded values, so an entity fetched by one key can be served from the cache by another.
- `[DataLoaderGroup("RecommendationBatchingContext")]` on the class groups its loaders into one generated context interface (`IRecommendationBatchingContext`) that services can inject as a unit.
- `ServiceScope`: by default each fetch gets a dedicated service scope. Leave the default unless the fetch genuinely must share the request scope (`OriginalScope`).

## Method shapes

The return type decides what kind of DataLoader is generated:

| Shape | Kind | Missing key resolves to |
| --- | --- | --- |
| `Task<Dictionary<TKey, TValue>>` | batch (1:1) | `null` |
| `Task<Dictionary<TKey, TValue[]>>` | grouped (1:n) | `null` — coalesce with `?? []` |
| `Task<TValue>` (single key parameter) | cache (per-key fetch, request-cached) | — |

A missing key must resolve, never throw — return a dictionary containing only the keys you found. A parent with no children resolves to `null` (the grouped shape is a batch loader whose value is an array), so 1:n consumers coalesce:

```csharp
=> await recommendationsByUserId.With(query).LoadAsync(user.Id, cancellationToken) ?? [];
```

If a missing key means broken data, the *consumer* decides by calling `LoadRequiredAsync`, which throws `KeyNotFoundException` naming the missing key. For 1:n relations that can grow unbounded, don't use these shapes — page through a batch-paging DataLoader instead (see [pagination.md](pagination.md)).

## Relations always resolve through a DataLoader

Any field on an entity type that fetches data uses a DataLoader. A relation resolver that queries the `DbContext` directly runs one query per parent — the textbook N+1.

**Wrong — direct query in a relation resolver:**

```csharp
[ObjectType<Recommendation>]
public static partial class RecommendationType
{
    public static async Task<User?> GetUserAsync(
        [Parent] Recommendation recommendation,
        RecommendationContext context,          // one query per Recommendation!
        CancellationToken cancellationToken)
        => await context.Users
            .FirstOrDefaultAsync(u => u.Id == recommendation.UserId, cancellationToken);
}
```

**Right — DataLoader injected as a resolver parameter:**

```csharp
[ObjectType<Recommendation>]
public static partial class RecommendationType
{
    public static async Task<User?> GetUserAsync(
        [Parent] Recommendation recommendation,
        QueryContext<User> query,
        UserByIdDataLoader userById,
        CancellationToken cancellationToken)
        => await userById.With(query).LoadAsync(recommendation.UserId, cancellationToken);
}
```

`.With(query)` hands the resolver's query context to the DataLoader. DataLoaders are immutable — `.With(...)` branches the loader keyed by the context state, so consumers with the same state share one batch and one cache.

## Top-level `byId` fields use DataLoaders too

It is tempting to query the database directly in a root resolver — "there is only one of it per request." That intuition is wrong: root lookups batch across aliases and across batched requests.

- **Alias batching:** one operation can request the same field many times — `a: recommendationById(id: 1) { … } b: recommendationById(id: 2) { … }`. With a DataLoader those coalesce into one `IN` query; without it, one query each.
- **Variable batching / request batching:** HotChocolate folds a transport batch (one query document with an array of variable sets, or an array of requests) into a single work scheduler, executing the entries as if they had been sent as one colocated request — overlapping lookups collapse into fewer database roundtrips. This is also how composite-schema gateways talk to source schemas: batched lookups against your `byId` fields.

**Right — root lookup through the DataLoader:**

```csharp
[QueryType]
internal static partial class RecommendationQueries
{
    [Lookup]
    public static async Task<Recommendation?> GetRecommendationByIdAsync(
        int id,
        QueryContext<Recommendation> query,
        RecommendationByIdDataLoader recommendationById,
        CancellationToken cancellationToken)
        => await recommendationById.With(query).LoadAsync(id, cancellationToken);
}
```

`LoadAsync` may return `null` when the key is not found — which is why this field is nullable (`Recommendation?`). For a non-null field use `LoadRequiredAsync` instead: it has a non-null return type and errors when the key cannot be resolved.

The `[Lookup]` attribute on the field is a subgraph concept — it marks the field as a fetcher for an entity by stable key; see [subgraph.md](subgraph.md). Its relevance here: gateways batch lookup fields hard, which is one more reason root `byId` fields must be DataLoader-backed.

## Consuming rules

- Inject the generated class as a **resolver method parameter** (`UserByIdDataLoader`, not `IUserByIdDataLoader` — the class navigates to your fetch code). DataLoaders are request-scoped; parameter injection gives you the request's instance. (Constructor injection into a scoped service is fine — never into a singleton.)
- Pass the resolver's `QueryContext<T>` along with `.With(query)` so projections reach the database.
- Match the load method to the field's nullability. `LoadAsync` returns `TValue?` — for nullable fields, where a missing key legitimately yields `null`. `LoadRequiredAsync` returns non-null `TValue` and errors when the key cannot be resolved — for non-null fields.
- Do not combine `QueryContext<T>` with `[UseProjection]` on the same field — both apply a selector, and the HC0099 analyzer flags the combination.

## Gotchas

- The key list passed into the fetch method is rented; copying it is the only safe way to keep it.
- A batch fetch must not throw for missing keys — return the found subset and let `LoadAsync`/`LoadRequiredAsync` semantics handle absence.
- Never pair `LoadAsync` with a non-null field: a `null` slipping through fails the field with `HC0018` and propagates null per GraphQL rules. Decide the field's nullability deliberately (see the `graphql-schema-design` skill), then pick the load method that matches.
- Fetch logic must stay a pure function of keys + query context: same inputs, same query. Per-request state beyond the declared parameters breaks batch sharing.
- Batching always means DataLoaders here. v16's `[BatchResolver]` is not part of these best practices — even if docs or samples show it, implement the field with a DataLoader.
