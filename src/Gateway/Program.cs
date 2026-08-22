using System.Text.Json.Serialization;
using HotChocolate.Adapters.OpenApi;
using HotChocolate.Fusion.Subscriptions.NATS;
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
    .ModifyRequestOptions(
        o =>
        {
            o.CollectOperationPlanTelemetry = true;
            o.AllowOperationPlanRequests = true;
        })
    .ModifyServerOptions(
        o =>
        {
            o.Tool.ServeMode = ChilliCream.Nitro.App.ServeMode.Insider;
        })
    // TODO: re-enable once fixed — Fusion 16.6.1 (through 16.6.2-p.6) AddInstrumentation
    // breaks all source schema fetches when an ActivityListener is active (OTel tracing):
    // every operation returns null data without errors and no subgraph request is sent.
    // .AddInstrumentation()
    .AddMcp()
    .AddOpenApi()
    .AddNatsEventStreamBroker(
        o =>
        {
            o.Url = builder.Configuration.GetConnectionString(Env.Nats);
            o.JetStream = new NatsJetStreamOptions
            {
                Stream = "reviews"
            };
        })
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
