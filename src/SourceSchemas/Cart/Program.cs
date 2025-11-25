using Demo.Cart.Data;
using Demo.Cart.Properties;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.CartApi, Env.Version)
    .AddNpgsqlDbContext<CartContext>(Env.CartDb);

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
    .AddGraphQL(Env.CartApi)
    .AddNitro()
    .AddAuthorization()
    .AddDefaultSettings(registerNodeInterface: false)
    .AddCartTypes()
    .AddWarmupTask(CartContext.SeedDataAsync, skipIf: args.IsGraphQLCommand());

var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();
app.RunWithGraphQLCommands(args);
