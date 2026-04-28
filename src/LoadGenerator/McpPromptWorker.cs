using ModelContextProtocol.Client;
using Microsoft.Extensions.Options;
using SharpYaml;

namespace LoadGenerator;

public sealed partial class McpPromptWorker(
    IHttpClientFactory httpClientFactory,
    IOptions<LoadGeneratorOptions> options,
    ILogger<McpPromptWorker> logger,
    ILoggerFactory loggerFactory,
    IHostEnvironment environment) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = options.Value;
        var workerConfig = config.McpPrompts;

        if (!workerConfig.Enabled)
        {
            LogDisabled(logger);
            return;
        }

        var prompts = LoadPrompts();

        if (prompts.Count == 0)
        {
            LogNoPrompts(logger);
            return;
        }

        LogPromptsLoaded(logger, prompts.Count);

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
                .Select(_ => prompts.SelectRandom())
                .ToList();

            var tasks = new Task[batch.Count];
            for (var i = 0; i < batch.Count; i++)
            {
                tasks[i] = ExecutePromptAsync(client, batch[i], semaphore, stoppingToken);
            }

            await Task.WhenAll(tasks);

            var delayMs = Random.Shared.Next(workerConfig.MinDelayMs, workerConfig.MaxDelayMs + 1);
            await Task.Delay(delayMs, stoppingToken);
        }
    }

    private async Task ExecutePromptAsync(
        McpClient client,
        PromptInfo prompt,
        SemaphoreSlim semaphore,
        CancellationToken ct)
    {
        await semaphore.WaitAsync(ct);
        try
        {
            var arguments = SelectArguments(prompt.SampleArguments);
            await client.GetPromptAsync(prompt.Name, arguments, cancellationToken: ct);
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

    private WeightedList<PromptInfo> LoadPrompts()
    {
        var path = Path.Combine(environment.ContentRootPath, "configuration", "prompts.yaml");

        if (!File.Exists(path))
        {
            path = Path.Combine(AppContext.BaseDirectory, "configuration", "prompts.yaml");
        }

        var yaml = File.ReadAllText(path);
        var entries = YamlSerializer.Deserialize<List<Dictionary<string, object?>>>(yaml)!;

        var items = new List<(PromptInfo, int)>();

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

            items.Add((new PromptInfo(name, sampleArguments), weight));
        }

        return new WeightedList<PromptInfo>(items);
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

    [LoggerMessage(Level = LogLevel.Information, Message = "MCP prompt load generator is disabled")]
    private static partial void LogDisabled(ILogger logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "No prompts found in prompts.yaml")]
    private static partial void LogNoPrompts(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Loaded {count} prompts")]
    private static partial void LogPromptsLoaded(ILogger logger, int count);

    private sealed record PromptInfo(
        string Name,
        Dictionary<string, object?[]> SampleArguments
    );
}
