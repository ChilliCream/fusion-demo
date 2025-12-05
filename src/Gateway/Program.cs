using Demo.Gateway.Mcp;
using HotChocolate.Adapters.Mcp.Extensions;
using HotChocolate.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
    .AddMcpStorage(new FileSystemMcpStorage("./Mcp"));

var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseHeaderPropagation();
app.UseAuthentication();
app.UseAuthorization();
app.MapGraphQL().WithOptions(new GraphQLServerOptions { Tool = {  ServeMode = GraphQLToolServeMode.Insider } });
app.MapGraphQLMcp();

app.Run();
