# Sorting

Client-controlled sorting is opt-in surface, exactly like filtering: every sortable field is a public contract and a query shape the database must serve efficiently. Sorting is added deliberately, never sprinkled.

## Rules

1. **Only use `UseSorting` when there is a genuine use-case for it.** Do not add `UseSorting` to all list and paged fields. Most fields are fine with the server-defined order (see [pagination.md](pagination.md)); a field gets client sorting because a client actually needs to reorder it.

2. **Be restrictive about what you allow sorting on.** Never expose the implicit default (every property). Define an explicit sort input type and bind fields explicitly — deciding per field whether sorting even makes sense (sorting by a description does not; sorting by what related objects hold, like the brand's name, often does):

   ```csharp
   public sealed class ProductSortInputType : SortInputType<Product>
   {
       protected override void Configure(ISortInputTypeDescriptor<Product> descriptor)
       {
           descriptor.BindFieldsExplicitly();

           descriptor.Field(t => t.Name);
           descriptor.Field(t => t.Price);
           descriptor.Field(t => t.Brand).Type<BrandSortInputType>();
       }
   }

   public sealed class BrandSortInputType : SortInputType<Brand>
   {
       protected override void Configure(ISortInputTypeDescriptor<Brand> descriptor)
       {
           descriptor.BindFieldsExplicitly();
           descriptor.Field(t => t.Name);
       }
   }
   ```

   Sorting by a related entity works by giving the navigation field its own restrictive sort input type — every related type gets the same explicit-binding treatment.

   Do not bind the type at the field level. The source generator picks up sort input types and binds them automatically — the field just declares `[UseSorting]`. Explicitly bind a type on a field (`[UseSorting<AlternativeProductSortInputType>]`) only when the same entity needs alternative sort inputs on different paths in the graph.

   Unlike filtering, there are no operations to restrict — a sortable field simply sorts ascending or descending. The restriction surface is *which* fields are sortable.

3. **Only allow sorting on columns backed by an index.** A sort becomes an `ORDER BY`; with cursor pagination it also becomes the seek path for every page fetch. An unindexed sort column means sorting the whole result set on every page.

## Interplay

- A client-supplied sort flows into resolvers and DataLoaders as the `Sorting` of `QueryContext<T>` — applied by the `.With(query)` pipeline (filter → sort → projection).
- On paginated fields the ordering rules from [pagination.md](pagination.md) still hold: pass `DefaultOrder` alongside the query context so a fallback order exists when the client doesn't sort, and the key is always appended as the final columns — a client sort never escapes the distinct-order requirement.
- Registering `AddSorting()` on the builder also registers `QueryContext<T>` support.

## Gotchas

- `BindFieldsExplicitly()` is as load-bearing here as in filter types — the implicit default exposes every property as sortable.
- Removing a sort field later is a breaking change; review sort fields like schema changes.
