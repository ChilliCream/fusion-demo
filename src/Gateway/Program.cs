using System.Text.Json.Serialization;
using ChilliCream.Nitro.App;
using HotChocolate.Adapters.Mcp.Extensions;
using HotChocolate.Adapters.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(Env.GatewayApi, Env.Version);

builder.Services
    .AddCors()
    .AddHeaderPropagation(c =>
    {
        c.Headers.Add("GraphQL-Preflight");
        c.Headers.Add("Authorization");
    });

builder.Services
    .AddHttpClient("fusion")
    .AddHeaderPropagation();

builder.Services
    .AddOpenApi(o => o.AddGraphQLTransformer());

builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.TypeInfoResolverChain.Add(ProblemDetailsJsonContext.Default);
});

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

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = null; // Allow anonymous by default
});

builder.Services.AddNitro().AddDefaults();

builder
    .AddGraphQLGateway()
    .ModifyRequestOptions(o => o.CollectOperationPlanTelemetry = true)
    .AddInstrumentation()
    .AddMcp()
    .AddOpenApi()
    .UsePersistedOperationPipeline();

var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseHeaderPropagation();
app.UseAuthentication();
app.UseAuthorization();
app.MapGraphQL();
app.MapGraphQLMcp();
app.MapOpenApiEndpoints();
app.MapOpenApi();
app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "eShop"));

app.RunWithGraphQLCommands(args);

[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(HttpValidationProblemDetails))]
internal sealed partial class ProblemDetailsJsonContext : JsonSerializerContext;
