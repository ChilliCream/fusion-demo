# Server Setup

A HotChocolate project always references the source generator and names its module explicitly; the server configuration splits general defaults from schema-specific choices so the main chain stays clear.

## Packages

Reference the source generator as an analyzer-only package:

```xml
<PackageReference Include="HotChocolate.AspNetCore" />
<PackageReference Include="HotChocolate.Data" />
<PackageReference Include="HotChocolate.Types.Analyzers">
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  <PrivateAssets>all</PrivateAssets>
</PackageReference>
```

## Module name

`Properties/ModuleInfo.cs` holds one line, naming the module after the domain — never rely on the assembly-name-derived default:

```csharp
[assembly: Module("ProductTypes")]
```

This specifies the name of the generated method that registers all GraphQL-related types of the project — types and DataLoaders alike: `AddProductTypes()`.

## Default settings extension

Move general server settings into a shared extension method so the main configuration of the schema stays clear:

```csharp
public static class Extensions
{
    private const string Production = nameof(Production);

    public static IRequestExecutorBuilder AddDefaultSettings(
        this IRequestExecutorBuilder builder,
        bool registerNodeInterface = true)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        builder.AddGlobalObjectIdentification(
            o =>
            {
                o.RegisterNodeInterface = registerNodeInterface;
                o.MarkNodeFieldAsLookup = true;
            });
        builder.AddInstrumentation();
        builder.AddMutationConventions();
        builder.AddPagingArguments();
        builder.AddQueryContext();
        builder.ModifyCostOptions(x => x.EnforceCostLimits = false);

        if (!Production.Equals(environmentName, StringComparison.OrdinalIgnoreCase))
        {
            builder.ExportSchemaOnStartup();
        }

        return builder;
    }
}
```

What the defaults establish:

- `AddGlobalObjectIdentification` — always on. Global Object Identification gives every entity a stable global ID and the relay `node` field, so clients can more easily refetch and cache entities; `MarkNodeFieldAsLookup = true` makes the `node` field act as a lookup (see [subgraph.md](subgraph.md)).
- `AddInstrumentation` — telemetry for the executor.
- `AddMutationConventions` — always on. The conventions generate the uniform mutation shape — input type, `{Name}Payload` type, and a typed `errors` field from `[Error(typeof(...))]` declarations — so mutations stay consistent without hand-written input/payload classes (see [mutations.md](mutations.md)).
- `AddPagingArguments` + `AddQueryContext` — the paging and projection primitives every resolver relies on (see [pagination.md](pagination.md) and [dataloaders.md](dataloaders.md)).
- `ModifyCostOptions(x => x.EnforceCostLimits = false)` — for subgraphs. The cost annotations are still added to the schema, but the subgraph does not enforce them — enforcement is up to the gateway (see [subgraph.md](subgraph.md)). A standalone server keeps enforcement on.
- `ExportSchemaOnStartup` outside production — keeps the exported schema file current for tooling and composition.

## Main chain

The main configuration then reads as the schema's identity — only schema-specific choices appear:

```csharp
builder
    .AddGraphQL(Env.ProductsApi)
    .ModifyPagingOptions(o => o.NullOrdering = NullOrdering.NativeNullsLast)
    .AddAuthorization()
    .AddDefaultSettings()
    .AddUploadType()
    .AddProductTypes();
```

Schema-specific choices — null ordering for paging, authorization, scalar/upload types, and the project's generated `Add{Module}()` — stay visible in the main chain; everything generic hides behind `AddDefaultSettings()`.
