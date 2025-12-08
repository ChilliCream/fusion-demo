using Demo.Gateway.Mcp;
using HotChocolate.Adapters.Mcp.Extensions;
using HotChocolate.Adapters.OpenApi;
using HotChocolate.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(Env.GatewayApi, Env.Version);

builder.Services
    .AddCors()
    .AddHeaderPropagation(c =>
    {
        c.Headers.Add("GraphQL-Preflight");
        c.Headers.Add("Authorization");
        c.Headers.Add("x-gateway-baseurl", v =>
        {
            var request = v.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}";
        });
    });

builder.Services
    .AddOpenApi(o => o.AddGraphQLTransformer());

builder.Services
    .AddHttpClient("fusion")
    .AddHeaderPropagation();

builder.Services
    .AddReverseProxy()
    .LoadFromMemory(
        [
            new()
            {
                RouteId = "images",
                ClusterId = "catalog",
                Match = new()
                {
                    Path = "/images/{**catch-all}"
                }
            }
        ],
        [
            new()
            {
                ClusterId = "catalog",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    ["catalog"] = new() { Address = "http://localhost:5110" }
                }
            }
        ]);

builder.Services.AddLogging();

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

builder
    .AddGraphQLGateway()
    // .AddFileSystemConfiguration("./gateway.far")
    .AddNitro(options => options.Metrics.Enabled = false)
    // .AddDiagnosticEventListener(c => new DebugDiagnosticListener(c.GetRequiredService<IRootServiceProviderAccessor>().ServiceProvider.GetRequiredService<ILoggerFactory>()))
    .ModifyRequestOptions(o => o.CollectOperationPlanTelemetry = true)
    .AddMcp()
    .AddMcpStorage(new FileSystemMcpStorage("./Mcp"))
    .AddOpenApiDefinitionStorage(new FileSystemOpenApiDefinitionStorage("./OpenApi"));

var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseHeaderPropagation();
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();
app.MapGraphQL().WithOptions(new GraphQLServerOptions { Tool = {  ServeMode = GraphQLToolServeMode.Insider } });
app.MapGraphQLMcp();
app.MapOpenApiEndpoints();
app.MapOpenApi();
app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "eShop"));

app.Run();
