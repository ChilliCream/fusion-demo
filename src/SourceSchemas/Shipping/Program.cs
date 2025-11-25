using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(Env.ShippingApi, Env.Version);

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
    .AddGraphQL(Env.ShippingApi)
    .AddNitro()
    .AddAuthorization()
    .AddDefaultSettings(registerNodeInterface: false)
    .AddShippingTypes();

var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseAuthentication();
app.UseAuthorization();
app.MapGraphQL();

app.RunWithGraphQLCommands(args);
