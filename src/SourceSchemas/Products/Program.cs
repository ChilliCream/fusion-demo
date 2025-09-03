var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.ProductsApi, Env.Version)
    .AddNpgsqlDbContext<ProductContext>(Env.ProductsDb);

builder.Services.AddCors();

builder
    .AddGraphQL(Env.ProductsApi)
    .AddNitro()
    .AddDefaultSettings()
    .AddUploadType()
    .AddProductTypes()
    .InitializeOnStartup(ProductContext.SeedDataAsync, skipIf: args.IsGraphQLCommand());

var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.MapGraphQL();

app.RunWithGraphQLCommands(args);
