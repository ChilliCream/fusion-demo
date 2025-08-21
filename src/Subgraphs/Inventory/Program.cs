var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContextPool<InventoryContext>(o => o.UseSqlite("Data Source=inventory.db"));

builder.AddServiceDefaults("Inventory-Subgraph", Env.Version);

builder.Services
    .AddGraphQLServer()
    .AddTypes()
    .AddUploadType()
    .AddGraphQLDefaults();

var app = builder.Build();

await DatabaseHelper.SeedDatabaseAsync(app);

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
