using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContextPool<ProductContext>(
        o => o.UseSqlite("Data Source=product.db"));

builder.Services
    .AddGraphQLServer()
    .AddTypes()
    .AddGlobalObjectIdentification()
    .RegisterDbContext<ProductContext>()
    .AddInstrumentation(o => o.RenameRootActivity = true);

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(b => b.AddService("Products-Subgraph", "Demo", Env.Version))
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

await DatabaseHelper.SeedDatabaseAsync(app);

app.MapGraphQL();

app.RunWithGraphQLCommands(args);