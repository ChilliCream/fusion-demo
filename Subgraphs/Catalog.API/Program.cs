using GreenDonut.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContextPool<CatalogContext>(
        o => o.UseNpgsql(builder.Configuration.GetConnectionString("CatalogDB")))
    .AddMigration<CatalogContext, CatalogContextSeed>();

builder
    .AddGraphQL("catalog")
    .AddTypes()
    .AddQueryContext()
    .AddPagingArguments()
    .AddFiltering()
    .AddMutationConventions()
    .AddInMemorySubscriptions()
    .InitializeOnStartup();

var app = builder.Build();

app.MapGraphQL(schemaName:  "catalog");

app.RunWithGraphQLCommands(args);
