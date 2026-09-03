---
name: hotchocolate-best-practices
description: HotChocolate 16.6+ server implementation best practices for .NET GraphQL services. Use when writing or reviewing C# in a HotChocolate project — resolvers, classes with [QueryType], [MutationType], [ObjectType], or [DataLoader], Green Donut DataLoaders, EF Core data access inside resolvers, pagination, or GraphQL performance work in .NET. Triggers on 'HotChocolate', 'Green Donut', 'DataLoader', 'resolver', 'AddGraphQLServer', or any edit to a file that uses HotChocolate attributes or types. Applies to standalone servers and to source schemas (subgraphs) in a composite setup.
---

# HotChocolate Best Practices

You are implementing or reviewing a HotChocolate GraphQL server in .NET. All guidance targets HotChocolate 16.6 or newer — do not apply it to v15-or-older codebases without checking the migration guides. This skill defines how ChilliCream expects that code to look. It covers implementation — resolvers, data access, DataLoaders, pagination. Schema *design* (SDL shape, naming, nullability) belongs to the sibling skill `graphql-schema-design`; use both when a task spans design and implementation.

## Core principles

1. **Implementation-first.** Write plain C# — static resolver classes with attributes like `[QueryType]`, `[MutationType]`, `[ObjectType<T>]`, `[DataLoader]` — and let HotChocolate infer the schema. Do not use schema-first SDL binding, and reach for descriptor/`Configure` overrides only for what attributes cannot express.

2. **All data access flows through DataLoaders.** A resolver for a relation (any non-root field that fetches data) never queries a `DbContext` or repository directly — it calls a DataLoader. Direct queries create N+1 loads and bypass request-scoped caching.

2. **Top-level `byId` fields use DataLoaders too.** Root lookups are batchable: clients batch them with field aliases in a single operation and with request/variable batching across operations. A root resolver that hits the database directly forfeits that coalescing.

3. **DataLoaders are source-generated.** Write a static method annotated with `[DataLoader]`; never hand-write a class deriving from `BatchDataLoader`/`GroupedDataLoader`. The generator emits the class, interface, and registration.

4. **Projections flow through `QueryContext<T>`.** Every DataLoader accepts a `QueryContext<TValue>` parameter so consumers can pass selections down to the database, and always pins its key into the projection (`query.Include(x => x.Id)`) so the key survives narrowing. The flip side: every resolver that reads parent properties declares them with `[Parent(requires: nameof(...))]` so projections keep those fields available.

5. **Pagination is cursor-based and ordered.** Paginated fields use `PagingArguments` + `ToPageAsync`/`ToBatchPageAsync`; every paginated query has a deterministic order ending in the key, and paging on non-root types goes through a batch-paging DataLoader. Never offset pagination — relative cursors cover jump-to-page needs.

## Project setup

Every HotChocolate project references the source generator as an analyzer-only package and names its module explicitly:

```xml
<PackageReference Include="HotChocolate.AspNetCore" />
<PackageReference Include="HotChocolate.Data" />
<PackageReference Include="HotChocolate.Types.Analyzers">
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  <PrivateAssets>all</PrivateAssets>
</PackageReference>
```

`Properties/ModuleInfo.cs` holds one line, naming the module after the domain — never rely on the assembly-name-derived default:

```csharp
[assembly: Module("RecommendationTypes")]
```

This specifies the name of the generated method that registers all GraphQL-related types of the project — types and DataLoaders alike. General server settings live in a shared `AddDefaultSettings` extension so the main chain shows only schema-specific choices — see [references/server-setup.md](references/server-setup.md):

```csharp
builder
    .AddGraphQL()
    .AddDefaultSettings()
    .AddRecommendationTypes();
```

## Reference file index

Detailed rules with worked wrong-vs-right examples. Load only what the current task touches.

| Reference | When to load |
| --- | --- |
| [references/dataloaders.md](references/dataloaders.md) | Any DataLoader, any resolver that fetches data, any N+1 concern, any `byId` field |
| [references/pagination.md](references/pagination.md) | Any paginated field, `[UseConnection]`, `PagingArguments`, connection/page types, sorting on collections |
| [references/resolvers.md](references/resolvers.md) | Extending entities with `[ObjectType<T>]`, `[BindMember]`, `[Parent(requires:)]`, root type classes, field naming |
| [references/subgraph.md](references/subgraph.md) | The project is a subgraph — detect via `.AddSourceSchemaDefaults()` in the builder chain or a `schema-settings.json` file. Covers `[Lookup]` on `byId` fields, entity references, cost enforcement, node-as-lookup |
| [references/server-setup.md](references/server-setup.md) | New projects, `Program.cs` / builder chains, csproj packages, `[assembly: Module]`, `AddDefaultSettings` |
| [references/mutations.md](references/mutations.md) | Any mutation, `[MutationType]`, mutation conventions, `[Error]`, payload/input shapes |
| [references/filtering.md](references/filtering.md) | Any `UseFiltering` usage, filter input types, deciding whether a field should be filterable |
| [references/sorting.md](references/sorting.md) | Any `UseSorting` usage, sort input types, deciding whether a field should be client-sortable |

More references will be added as the skill grows.
