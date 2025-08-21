var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContextPool<ProductContext>(o => o.UseSqlite("Data Source=product.db"));

builder.AddServiceDefaults("Products-Subgraph", Env.Version);

builder.Services
    .AddGraphQLServer()
    .AddTypes()
    .AddUploadType()
    .AddGraphQLDefaults();

var app = builder.Build();

await DatabaseHelper.SeedDatabaseAsync(app);

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
