var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContextPool<OrderContext>(o => o.UseSqlite("Data Source=order.db"));

builder.AddServiceDefaults("Order-Subgraph", Env.Version);

builder.Services
    .AddGraphQLServer()
    .AddTypes()
    .AddUploadType()
    .AddGraphQLDefaults();

var app = builder.Build();

await DatabaseHelper.SeedDatabaseAsync(app);

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
