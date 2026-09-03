# Mutations

Mutations run through the mutation conventions — `AddMutationConventions()` is part of the default settings (see [server-setup.md](server-setup.md)) and always on. The conventions generate the uniform mutation shape so you never hand-write input or payload classes: each mutation field gets an input type from its parameters and a `{Name}Payload` type around its result, plus a typed `errors` field for declared domain errors.

## Shape

A mutation is a static method on a `[MutationType]` class, one class per domain:

```csharp
[MutationType]
internal static partial class RecommendationMutations
{
    [Error(typeof(RecommendationNotFoundException))]
    public static async Task<Recommendation> PublishRecommendationAsync(
        [ID<Recommendation>] int id,
        RecommendationService recommendationService,
        CancellationToken cancellationToken)
        => await recommendationService.PublishAsync(id, cancellationToken);
}
```

The conventions turn this into:

```graphql
type Mutation {
  publishRecommendation(input: PublishRecommendationInput!): PublishRecommendationPayload!
}
```

- Only parameters that map to GraphQL arguments appear in the generated input — services and `CancellationToken` are resolver concerns and stay out.
- The returned entity becomes the payload's data field.
- `[ID<T>]` on key arguments keeps global object identification intact through mutations.

## Errors

Declare expected domain errors with `[Error(typeof(...))]` on the mutation. Declared exceptions surface as typed members of the payload's `errors` field instead of a generic top-level GraphQL error — clients can match on them. Undeclared exceptions remain unexpected failures.

## Naming

Every mutation has a clear verb (`publishRecommendation`, not `updateRecommendation`) — the schema-design rules for mutation granularity, verbs, and error modeling live in the sibling `graphql-schema-design` skill; this reference covers the implementation side.

## Gotchas

- Don't hand-write input/payload records for convention-managed mutations — the duplication drifts.
- Don't throw plain exceptions for expected failure cases; declare them via `[Error(typeof(...))]` so they become part of the contract.
