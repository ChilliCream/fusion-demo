# Filtering

Filtering is opt-in surface, not a default. Every filterable field is a public contract *and* a query shape the database must serve efficiently forever — so filtering is added deliberately, never sprinkled.

## Rules

1. **Only use `UseFiltering` when there is a genuine use-case for it.** Do not add `UseFiltering` to all list and paged fields. A field gets filtering because a client needs a specific capability, not because the field returns a collection.

2. **Be restrictive about what you allow filters on.** Never expose the implicit default (every property, nested objects included). Define an explicit filter input type and bind fields explicitly:

   ```csharp
   public sealed class ProductFilterInputType : FilterInputType<Product>
   {
       protected override void Configure(IFilterInputTypeDescriptor<Product> descriptor)
       {
           descriptor.BindFieldsExplicitly();

           descriptor.Field(t => t.Name);
           descriptor.Field(t => t.Type);
           descriptor.Field(t => t.Brand);
           descriptor.Field(t => t.Price);
           descriptor.Field(t => t.AvailableStock);
       }
   }
   ```

   Do not bind the type at the field level. The source generator picks up filter input types and binds them automatically — the field just declares `[UseFiltering]`. Explicitly bind a type on a field (`[UseFiltering<AlternativeProductFilterInputType>]`) only when the same entity needs alternative filter inputs on different paths in the graph.

3. **Only allow filtering on columns backed by an index.** A filter becomes a `WHERE` clause; an unindexed column means a scan that gets slower as the table grows. If a field is worth filtering on, it is worth an index — add the index or don't expose the filter.

4. **Restrict the default string filter.** Out of the box, every string filter field exposes the full operation set — restrict the defaults so all string fields share a deliberate, lean operation list:

   ```csharp
   public sealed class DefaultStringOperationFilterInputType : StringOperationFilterInputType
   {
       protected override void Configure(IFilterInputTypeDescriptor descriptor)
       {
           descriptor.Operation(DefaultFilterOperations.Equals).Type<StringType>();
           descriptor.Operation(DefaultFilterOperations.StartsWith).Type<StringType>();
       }
   }
   ```

   Register it with the filter provider as the runtime binding for `string`:

   ```csharp
   services
       .AddGraphQLServer()
       .AddFiltering(
           c => c.AddDefaults()
               .BindRuntimeType<string, DefaultStringOperationFilterInputType>())
   ```

   Which operations to allow depends on your use-cases (the two above are an example) — but pick operations indexes can serve: `equals` and `startsWith` are index-friendly, while `contains`/`endsWith` translate to `LIKE '%…'` patterns that force scans.

   Fields that genuinely need richer operations opt out of the restricted default on a field-by-field basis, by giving that field a less restrictive operation type in the filter input:

   ```csharp
   public sealed class ProductFilterInputType : FilterInputType<Product>
   {
       protected override void Configure(IFilterInputTypeDescriptor<Product> descriptor)
       {
           descriptor.BindFieldsExplicitly();

           descriptor.Field(t => t.Name).Type<SearchStringOperationFilterInputType>();
           descriptor.Field(t => t.Type);
           descriptor.Field(t => t.Brand);
           descriptor.Field(t => t.Price);
           descriptor.Field(t => t.AvailableStock);
       }
   }
   ```

   `SearchStringOperationFilterInputType` is another `StringOperationFilterInputType` subclass allowing the richer operations that one field needs — the restrictive default stays in force for every other string field.

## Interplay

- Client filters flow into resolvers and DataLoaders as the `Predicate` of `QueryContext<T>` — the `.With(query)` pipeline from [dataloaders.md](dataloaders.md) applies them; nothing extra is needed in the resolver.
- Registering `AddFiltering()` on the builder also registers `QueryContext<T>` support.
- On paginated fields, filtering composes with the ordering rules from [pagination.md](pagination.md) unchanged — filter first, then the keyed order, then the projection.

## Gotchas

- `BindFieldsExplicitly()` is the load-bearing line — without it the filter input exposes every property of the entity, including navigations, and clients can construct arbitrarily deep and expensive filter expressions.
- Review filter fields like schema changes: removing one later is a breaking change.
