using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults(Env.AccountsApi, Env.Version)
    .AddNpgsqlDbContext<AccountContext>(Env.AccountsDb);

builder.Services.AddCors();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var keycloakUrl = builder.Configuration["Keycloak:Authority"] ?? "http://localhost:8080";
        options.Authority = $"{keycloakUrl}/realms/fusion-demo";
        options.Audience = "graphql-api";
        options.RequireHttpsMetadata = false; // For development only
        options.TokenValidationParameters = new()
        {
            ValidateAudience = false, // Keycloak doesn't always include audience
            ValidateIssuer = true,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

builder
    .AddGraphQL(Env.AccountsApi)
    .AddNitro()
    .AddAuthorization()
    .AddDefaultSettings()
    .AddAccountTypes();

var app = builder.Build();

if (!args.IsGraphQLCommand())
{
    await AccountContext.SeedDataAsync(app.Services);
}

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseAuthentication();
app.UseAuthorization();
app.MapGraphQL();

app.RunWithGraphQLCommands(args);
