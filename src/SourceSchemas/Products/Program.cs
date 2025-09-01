var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.ProductsApi, Env.Version)
    .AddNpgsqlDbContext<ProductContext>(Env.ProductsDb);

builder.Services.AddCors();

builder
    .AddGraphQL(Env.ProductsApi)
    .AddDefaultSettings()
    .AddUploadType()
    .AddProductTypes()
    .InitializeOnStartup(ProductContext.SeedDataAsync);

var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.MapGraphQL();

app.RunWithGraphQLCommands(args);
