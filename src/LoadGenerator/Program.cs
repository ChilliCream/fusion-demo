using OpenTelemetry;
using OpenTelemetry.Log;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
});

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(r => r.AddService(Env.LoadGenerator, "Demo", Env.Version))
    .WithMetrics(metrics =>
    {
        metrics
            .AddHttpClientInstrumentation()
            .AddNitroExporter();
    })
    .WithTracing(tracing =>
    {
        tracing
            .AddHttpClientInstrumentation()
            .AddNitroExporter();
    })
    .WithLogging(logging =>
    {
        logging.AddNitroExporter();
    });

if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
{
    builder.Services.AddOpenTelemetry().UseOtlpExporter();
}

builder.Services.Configure<LoadGeneratorOptions>(
    builder.Configuration.GetSection(LoadGeneratorOptions.SectionName));

builder.Services.AddHttpClient("GraphQL", (_, client) =>
{
    var options = builder.Configuration
        .GetSection(LoadGeneratorOptions.SectionName)
        .Get<LoadGeneratorOptions>() ?? new LoadGeneratorOptions();
    client.BaseAddress = new Uri(options.GatewayUrl.TrimEnd('/') + "/");
});

builder.Services.AddHttpClient("Mcp");

builder.Services.AddHttpClient("OpenApi", (_, client) =>
{
    var options = builder.Configuration
        .GetSection(LoadGeneratorOptions.SectionName)
        .Get<LoadGeneratorOptions>() ?? new LoadGeneratorOptions();
    client.BaseAddress = new Uri(options.GatewayUrl.TrimEnd('/') + "/");
});

builder.Services.AddHostedService<GraphQLWorker>();
builder.Services.AddHostedService<McpWorker>();
builder.Services.AddHostedService<OpenApiWorker>();

var host = builder.Build();
host.Run();
