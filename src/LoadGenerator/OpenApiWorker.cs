using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using SharpYaml;

namespace LoadGenerator;

public sealed partial class OpenApiWorker(
    IHttpClientFactory httpClientFactory,
    IOptions<LoadGeneratorOptions> options,
    ILogger<OpenApiWorker> logger,
    IHostEnvironment environment) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var workerConfig = options.Value.OpenApi;

        if (!workerConfig.Enabled)
        {
            LogDisabled(logger);
            return;
        }

        var endpoints = LoadEndpoints();

        if (endpoints.Count == 0)
        {
            LogNoEndpoints(logger);
            return;
        }

        LogEndpointsLoaded(logger, endpoints.Count);

        var httpClient = httpClientFactory.CreateClient("OpenApi");
        using var semaphore = new SemaphoreSlim(workerConfig.MaxConcurrentRequests);

        while (!stoppingToken.IsCancellationRequested)
        {
            var batchSize = Random.Shared.Next(1, workerConfig.MaxRequestsPerBatch + 1);
            var batch = Enumerable.Range(0, batchSize)
                .Select(_ => endpoints.SelectRandom())
                .ToList();

            var tasks = new Task[batch.Count];
            for (var i = 0; i < batch.Count; i++)
            {
                tasks[i] = ExecuteRequestAsync(httpClient, batch[i], semaphore, stoppingToken);
            }

            await Task.WhenAll(tasks);

            var delayMs = Random.Shared.Next(workerConfig.MinDelayMs, workerConfig.MaxDelayMs + 1);
            await Task.Delay(delayMs, stoppingToken);
        }
    }

    private async Task ExecuteRequestAsync(
        HttpClient httpClient,
        EndpointInfo endpoint,
        SemaphoreSlim semaphore,
        CancellationToken ct)
    {
        await semaphore.WaitAsync(ct);
        try
        {
            var parameters = SelectParameters(endpoint.SampleParameters);
            var uri = BuildUri(endpoint.Route, parameters);
            using var response = await httpClient.GetAsync(uri, ct);
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

    private static string BuildUri(string route, Dictionary<string, object?> parameters)
    {
        var path = PathPlaceholderRegex().Replace(route, match =>
        {
            var key = match.Groups[1].Value;
            if (parameters.Remove(key, out var value) && value is not null)
            {
                return Uri.EscapeDataString(value.ToString()!);
            }

            return match.Value;
        });

        var queryParams = parameters
            .Where(p => p.Value is not null)
            .Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value!.ToString()!)}");

        var query = string.Join("&", queryParams);
        return query.Length > 0 ? $"{path}?{query}" : path;
    }

    private WeightedList<EndpointInfo> LoadEndpoints()
    {
        var path = Path.Combine(environment.ContentRootPath, "configuration", "endpoints.yaml");

        if (!File.Exists(path))
        {
            path = Path.Combine(AppContext.BaseDirectory, "configuration", "endpoints.yaml");
        }

        var yaml = File.ReadAllText(path);
        var entries = YamlSerializer.Deserialize<List<Dictionary<string, object?>>>(yaml)!;

        var items = new List<(EndpointInfo, int)>();

        foreach (var entry in entries)
        {
            var route = (string)entry["route"]!;
            var weight = entry.TryGetValue("weight", out var w) ? Convert.ToInt32(w) : 1;

            var sampleParameters = new Dictionary<string, object?[]>();
            if (entry.TryGetValue("parameters", out var parametersObj)
                && parametersObj is Dictionary<string, object?> parameters)
            {
                foreach (var (key, value) in parameters)
                {
                    sampleParameters[key] = ((List<object?>)value!).ToArray();
                }
            }

            items.Add((new EndpointInfo(route, sampleParameters), weight));
        }

        return new WeightedList<EndpointInfo>(items);
    }

    private static Dictionary<string, object?> SelectParameters(
        Dictionary<string, object?[]> sampleParameters)
    {
        var dict = new Dictionary<string, object?>();

        foreach (var (name, samples) in sampleParameters)
        {
            dict[name] = samples[Random.Shared.Next(samples.Length)];
        }

        return dict;
    }

    [GeneratedRegex(@"\{(\w+)\}")]
    private static partial Regex PathPlaceholderRegex();

    [LoggerMessage(Level = LogLevel.Information, Message = "OpenAPI load generator is disabled")]
    private static partial void LogDisabled(ILogger logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "No endpoints found in endpoints.yaml")]
    private static partial void LogNoEndpoints(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Loaded {count} endpoints")]
    private static partial void LogEndpointsLoaded(ILogger logger, int count);

    private sealed record EndpointInfo(
        string Route,
        Dictionary<string, object?[]> SampleParameters
    );
}
