using ModelContextProtocol.Client;
using Microsoft.Extensions.Options;
using SharpYaml;

namespace LoadGenerator;

public sealed partial class McpWorker(
    IHttpClientFactory httpClientFactory,
    IOptions<LoadGeneratorOptions> options,
    ILogger<McpWorker> logger,
    ILoggerFactory loggerFactory,
    IHostEnvironment environment) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = options.Value;
        var workerConfig = config.Mcp;

        if (!workerConfig.Enabled)
        {
            LogDisabled(logger);
            return;
        }

        var tools = LoadTools();

        if (tools.Count == 0)
        {
            LogNoTools(logger);
            return;
        }

        LogToolsLoaded(logger, tools.Count);

        var mcpEndpoint = config.GatewayUrl.TrimEnd('/') + "/graphql/mcp";

        var httpClient = httpClientFactory.CreateClient("Mcp");
        var transport = new HttpClientTransport(
            new HttpClientTransportOptions
            {
                Endpoint = new Uri(mcpEndpoint),
                Name = "LoadGenerator"
            },
            httpClient,
            loggerFactory);

        await using var client = await McpClient.CreateAsync(
            transport,
            loggerFactory: loggerFactory,
            cancellationToken: stoppingToken);

        using var semaphore = new SemaphoreSlim(workerConfig.MaxConcurrentRequests);

        while (!stoppingToken.IsCancellationRequested)
        {
            var batchSize = Random.Shared.Next(1, workerConfig.MaxRequestsPerBatch + 1);
            var batch = Enumerable.Range(0, batchSize)
                .Select(_ => tools.SelectRandom())
                .ToList();

            var tasks = new Task[batch.Count];
            for (var i = 0; i < batch.Count; i++)
            {
                tasks[i] = ExecuteToolAsync(client, batch[i], semaphore, stoppingToken);
            }

            await Task.WhenAll(tasks);

            var delayMs = Random.Shared.Next(workerConfig.MinDelayMs, workerConfig.MaxDelayMs + 1);
            await Task.Delay(delayMs, stoppingToken);
        }
    }

    private async Task ExecuteToolAsync(
        McpClient client,
        ToolInfo tool,
        SemaphoreSlim semaphore,
        CancellationToken ct)
    {
        await semaphore.WaitAsync(ct);
        try
        {
            var arguments = SelectArguments(tool.SampleArguments);
            await client.CallToolAsync(tool.Name, arguments, cancellationToken: ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // Shutting down
        }
        catch
        {
            // Errors are reported via OTel
        }
        finally
        {
            semaphore.Release();
        }
    }

    private WeightedList<ToolInfo> LoadTools()
    {
        var path = Path.Combine(environment.ContentRootPath, "configuration", "tools.yaml");

        if (!File.Exists(path))
        {
            path = Path.Combine(AppContext.BaseDirectory, "configuration", "tools.yaml");
        }

        var yaml = File.ReadAllText(path);
        var entries = YamlSerializer.Deserialize<List<Dictionary<string, object?>>>(yaml)!;

        var items = new List<(ToolInfo, int)>();

        foreach (var entry in entries)
        {
            var name = (string)entry["name"]!;
            var weight = entry.TryGetValue("weight", out var w) ? Convert.ToInt32(w) : 1;

            var sampleArguments = new Dictionary<string, object?[]>();
            if (entry.TryGetValue("arguments", out var argumentsObj)
                && argumentsObj is Dictionary<string, object?> arguments)
            {
                foreach (var (key, value) in arguments)
                {
                    sampleArguments[key] = ((List<object?>)value!).ToArray();
                }
            }

            items.Add((new ToolInfo(name, sampleArguments), weight));
        }

        return new WeightedList<ToolInfo>(items);
    }

    private static Dictionary<string, object?> SelectArguments(
        Dictionary<string, object?[]> sampleArguments)
    {
        var dict = new Dictionary<string, object?>();

        foreach (var (name, samples) in sampleArguments)
        {
            dict[name] = samples[Random.Shared.Next(samples.Length)];
        }

        return dict;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "MCP load generator is disabled")]
    private static partial void LogDisabled(ILogger logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "No tools found in tools.yaml")]
    private static partial void LogNoTools(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Loaded {count} tools")]
    private static partial void LogToolsLoaded(ILogger logger, int count);

    private sealed record ToolInfo(
        string Name,
        Dictionary<string, object?[]> SampleArguments
    );
}
