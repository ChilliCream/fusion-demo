using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var config = new Configuration();
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCors()
    .AddHeaderPropagation(c =>
    {
        c.Headers.Add("GraphQL-Preflight");
        c.Headers.Add("Authorization");
    });

builder.Services
    .AddHttpClient("Fusion")
    .AddHeaderPropagation();

builder.Services
    .AddWebSocketClient();

builder.Services
    .AddFusionGatewayServer()
    .ConfigureFromCloud(
        b =>
        {
            b.ApiId = config.ApiId;
            b.ApiKey = config.ApiKey;
            b.Stage = config.Stage;
        })
    .CoreBuilder
    .AddInstrumentation(o => o.RenameRootActivity = true);

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(b => b.AddService("Shop-Gateway", "Demo", config.Version))
    .WithTracing(
        b =>
        {
            b.AddHttpClientInstrumentation();
            b.AddAspNetCoreInstrumentation();
            b.AddHotChocolateInstrumentation();
            b.AddOtlpExporter();
        })
    .WithMetrics(
        b =>
        {
            b.AddHttpClientInstrumentation();
            b.AddAspNetCoreInstrumentation();
            b.AddOtlpExporter();
        });

var app = builder.Build();

app.UseWebSockets();
app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseHeaderPropagation();
app.MapGraphQL();

app.RunWithGraphQLCommands(args);