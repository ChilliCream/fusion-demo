using Microsoft.Extensions.FileProviders;

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
    .AddWarmupTask(ProductContext.SeedDataAsync, skipIf: args.IsGraphQLCommand());

var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        System.IO.Path.Combine(builder.Environment.ContentRootPath, "images")),
    RequestPath = "/images"
});

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
