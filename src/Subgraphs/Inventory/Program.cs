var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.InventoryApi, Env.Version)
    .AddNpgsqlDbContext<InventoryContext>(Env.InventoryDb);

builder
    .AddGraphQL(Env.InventoryApi)
    .AddSubgraphDefaults()
    .AddInventoryTypes()
    .InitializeOnStartup(InventoryContext.SeedDatabaseAsync);

var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
