var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.InventoryApi, Env.Version)
    .AddNpgsqlDbContext<InventoryContext>(Env.InventoryDb);

builder
    .AddGraphQL()
    .AddDefaultSettings()
    .AddInventoryTypes()
    .InitializeOnStartup(InventoryContext.SeedDataAsync);

var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
