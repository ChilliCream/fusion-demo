using GreenDonut.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.ProductsApi, Env.Version)
    .AddNpgsqlDbContext<ProductContext>(Env.ProductsDb);

builder.Services.AddCors();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var keycloakUrl = builder.Configuration["Keycloak:Authority"] ?? "http://localhost:8080";
        options.Authority = $"{keycloakUrl}/realms/fusion-demo";
        options.Audience = "graphql-api";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new()
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

builder
    .AddGraphQL(Env.ProductsApi)
    .ModifyPagingOptions(o => o.NullOrdering = NullOrdering.NativeNullsLast)
    .AddNitro()
    .AddAuthorization()
    .AddDefaultSettings()
    .AddUploadType()
    .AddProductTypes();

var app = builder.Build();

if (!args.IsGraphQLCommand())
{
    await ProductContext.SeedDataAsync(app.Services);
}

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        System.IO.Path.Combine(builder.Environment.ContentRootPath, "images")),
    RequestPath = "/images"
});

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
