# Resolvers & Type Extensions

Implementation-first keeps domain models free of API concerns: plain C# entities become GraphQL object types automatically, and everything GraphQL-specific — relation fields, computed fields, member bindings — lives in a separate extension class. Nullable reference types drive schema nullability, so keep NRT enabled.

## Extending an entity: `[ObjectType<T>]`

To extend a domain model/entity for GraphQL, declare an `internal static partial class` named `{Entity}Node` and annotate it with `[ObjectType<T>]`:

```csharp
[ObjectType<Review>]
internal static partial class ReviewNode
{
    [BindMember(nameof(Review.ProductId))]
    public static Product GetProduct(
        [Parent(requires: nameof(Review.ProductId))] Review review)
        => new(review.ProductId);

    [BindMember(nameof(Review.AuthorId))]
    public static async Task<User?> GetAuthorAsync(
        [Parent(requires: nameof(Review.AuthorId))] Review review,
        UserByIdDataLoader userById,
        CancellationToken cancellationToken)
        => await userById.LoadAsync(review.AuthorId, cancellationToken);
}
```

Every piece is a rule:

- **`static partial` is mandatory.** The generator wires the class up at build time and enforces both keywords with compile errors (HC0080/HC0081).
- **`[BindMember(nameof(Entity.FkId))]` removes the bound member from the GraphQL model and adds the resolver in its place.** `AuthorId` never appears in the schema; the `author` field stands where it was — clients get the relation, never the bare key column. `nameof` keeps the binding refactoring-safe, and the analyzer validates the member exists on the entity (HC0094/HC0095). Binding also keeps the backing member in the queryable projection.
- **`[Parent(requires: nameof(Entity.FkId))]` declares what the resolver reads.** When `QueryContext<T>` is used across the solution, projections are always in play: the parent instance only contains the columns the selection needed. Specify which fields a resolver requires so they are available under projection — without `requires`, `review.ProductId` may be default/empty when the client didn't ask for it, and the resolver silently produces wrong results. Declare every parent property the resolver touches, on every resolver.
- **Match the fetch to the target:**
  - The target lives in this service → load it through a DataLoader (see [dataloaders.md](dataloaders.md)). Nullability pairing applies: `LoadAsync` for a nullable field (`User?`), `LoadRequiredAsync` for a non-null field.
  - The target is identified by its key alone — a reference to an entity another service owns, or a type constructible from the key — → construct it directly (`new Product(review.ProductId)`). No fetch, no DataLoader, synchronous.

**Wrong — reading a parent property without declaring it:**

```csharp
public static async Task<User?> GetAuthorAsync(
    [Parent] Review review,                      // no requires:
    UserByIdDataLoader userById,
    CancellationToken cancellationToken)
    => await userById.LoadAsync(review.AuthorId, cancellationToken);
    // review.AuthorId is 0 when the client didn't select it — wrong user, no error
```

This is the silent projection failure: everything works in tests that select all fields and breaks in production queries that don't.

## Root types

Root fields follow the same shape — `[QueryType]` / `[MutationType]` on an `internal static partial class` per domain:

```csharp
[QueryType]
internal static partial class ReviewQueries
{
    [Lookup]
    public static async Task<Review?> GetReviewByIdAsync(
        int id,
        QueryContext<Review> query,
        ReviewByIdDataLoader reviewById,
        CancellationToken cancellationToken)
        => await reviewById.With(query).LoadAsync(id, cancellationToken);
}
```

- Multiple `[QueryType]` classes merge into one `Query` type — one class per domain area, never one giant class.
- Field names are inferred: `Get` prefix and `Async` suffix are stripped, then camelCased (`GetReviewByIdAsync` → `reviewById`). Override only with `[GraphQLName]` when the convention cannot express the name.
- Resolver parameters that are not services, `CancellationToken`, `[Parent]`, or data-integration types (`QueryContext<T>`, `PagingArguments`) become GraphQL arguments.
- `byId` fields go through DataLoaders — see [dataloaders.md](dataloaders.md) — and carry `[Lookup]` (see [subgraph.md](subgraph.md)).

## Descriptor escape hatch

Attributes cover almost everything. For the rest, implement the generated `Configure` partial method — for example to hide an entity property that must not appear in the schema:

```csharp
[ObjectType<Review>]
internal static partial class ReviewNode
{
    static partial void Configure(IObjectTypeDescriptor<Review> descriptor)
        => descriptor.Ignore(t => t.InternalScore);
}
```

Reach for the descriptor only for what attributes cannot express — it is the exception, not the style.

## Gotchas

- A resolver on an extension class that reads a parent member without `requires` (or a `[BindMember]` binding) is the classic projection bug — wrong data, no exception.
- `[ExtendObjectType<T>]` is the legacy form; use `[ObjectType<T>]` (the analyzer suggests the upgrade, HC0096).
- Don't put GraphQL attributes on the domain entity itself when an extension class can carry them — the entity stays reusable outside the API.
