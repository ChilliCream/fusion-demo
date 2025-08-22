var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.ProductApi, Env.Version)
    .AddNpgsqlDbContext<ProductContext>(Env.ProductDb);

builder
    .AddGraphQL(Env.ProductApi)
    .AddDefaultSettings()
    .AddUploadType()
    .AddProductTypes()
    .InitializeOnStartup(ProductContext.SeedDataAsync);

var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
