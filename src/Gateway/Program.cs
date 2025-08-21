using Azure.Identity;
using Azure.Storage.Blobs;
using ChilliCream.Nitro.Azure;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults("Demo", App.Version);

builder.Services
    .AddOptions<StandaloneOtelExporterOptions>()
    .Bind(builder.Configuration.GetSection("StandaloneOtel"));

builder.Services
    .AddCors()
    .AddHeaderPropagation(c =>
    {
        c.Headers.Add("GraphQL-Preflight");
        c.Headers.Add("Authorization");
    });

builder.Services.AddHttpClient("Fusion").AddHeaderPropagation();

builder.Services.AddWebSocketClient();

builder.Services
    .AddFusionGatewayServer()
    .ConfigureFromCloud(x => x.Metrics.Enabled = true)
    .AddBlobStorageAssetCache(x =>
    {
        x.ContainerName = "fusion-demo-gateway";
        x.Client = new BlobServiceClient(
            new Uri("https://cccus2demodsaassetcache2.blob.core.windows.net/"),
            new DefaultAzureCredential());
    })
    .ModifyFusionOptions(o => o.AllowQueryPlan = true)
    .UsePersistedOperationPipeline()
    .CoreBuilder
    .ModifyOptions(x => x.EnableTag = false)
    .AddInstrumentation()
    .InitializeOnStartup();

builder.Services.ConfigureOpenTelemetryTracerProvider(x => x.AddStandaloneOtelExporter());

var app = builder.Build();

app.UseWebSockets();
app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseHeaderPropagation();
app.MapGraphQLPersistedOperations();
app.MapGraphQL();

app.RunWithGraphQLCommands(args);
