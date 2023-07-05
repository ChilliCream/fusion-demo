var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContextPool<ProductContext>(
        o => o.UseSqlite("Data Source=product.db"));

builder.Services
    .AddGraphQLServer()
    .AddTypes()
    .AddGlobalObjectIdentification()
    .RegisterDbContext<ProductContext>();

var app = builder.Build();

await DatabaseHelper.SeedDatabaseAsync(app);

app.MapGraphQL();

app.RunWithGraphQLCommands(args);