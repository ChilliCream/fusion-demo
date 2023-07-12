using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using static HotChocolate.WellKnownContextData;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(b => b.AddService("Shipping-Subgraph", "Demo", Env.Version))
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

builder.Services
    .AddGraphQLServer()
    .AddTypes()
    .ConfigureSchema(b => b.SetContextData(GlobalIdSupportEnabled, 1))
    .AddInstrumentation(o => o.RenameRootActivity = true);

var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
