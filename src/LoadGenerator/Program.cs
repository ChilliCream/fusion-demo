using LoadGenerator;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<LoadGeneratorOptions>(
    builder.Configuration.GetSection(LoadGeneratorOptions.SectionName));

builder.Services.AddHttpClient("GraphQL", (_, client) =>
{
    var options = builder.Configuration
        .GetSection(LoadGeneratorOptions.SectionName)
        .Get<LoadGeneratorOptions>() ?? new LoadGeneratorOptions();
    client.BaseAddress = new Uri(options.GatewayUrl.TrimEnd('/') + "/");
});

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