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
        // AppHost now wires Keycloak__Authority for cart-api the same way it does for
        // gateway-api; this https fallback only matters when running outside Aspire.
        var keycloakUrl = builder.Configuration["Keycloak:Authority"] ?? "https://localhost:8080";
        options.Authority = $"{keycloakUrl}/realms/fusion-demo";
        options.Audience = "graphql-api";
        options.RequireHttpsMetadata = false; // For development only
        options.TokenValidationParameters = new()
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidateLifetime = true
        };

        if (builder.Environment.IsDevelopment())
        {
            // Aspire's Keycloak resource is only reachable over its HTTPS dev-proxy, so
            // the wired authority is https despite the "http" endpoint name above.
            // Accept its self-signed local dev certificate when fetching the OIDC
            // metadata over https (see task fusion-demo-yt-dmm.6 comment).
            options.BackchannelHttpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
        }
    });

builder.Services.AddAuthorization();

builder.Services.AddNitro().AddHotChocolate(Env.CartApi);

builder
    .AddGraphQL(Env.CartApi)
    .AddAuthorization()
    .AddDefaultSettings(registerNodeInterface: false)
    .AddCartTypes();

var app = builder.Build();

if (!args.IsGraphQLCommand())
{
    await CartContext.SeedDataAsync(app.Services);
}

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();
app.RunWithGraphQLCommands(args);
