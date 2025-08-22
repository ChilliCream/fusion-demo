var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.ProductsApi, Env.Version)
    .AddNpgsqlDbContext<ProductContext>(Env.ProductsDb);

builder
    .AddGraphQL(Env.ProductsApi)
    .AddDefaultSettings()
    .AddUploadType()
    .AddProductTypes()
    .InitializeOnStartup(ProductContext.SeedDataAsync);

var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
