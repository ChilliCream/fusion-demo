using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

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

            LogSendingRequest(logger, uri);

            var sw = Stopwatch.StartNew();
            using var response = await httpClient.GetAsync(uri, ct);
            sw.Stop();

            if (response.IsSuccessStatusCode)
            {
                LogRequestSuccess(logger, uri, sw.ElapsedMilliseconds);
            }
            else
            {
                LogHttpError(logger, uri, sw.ElapsedMilliseconds, (int)response.StatusCode);
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // Shutting down
        }
        catch (Exception ex)
        {
            LogRequestException(logger, ex, endpoint.Route);
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
        var path = Path.Combine(environment.ContentRootPath, "configuration", "endpoints.json");

        if (!File.Exists(path))
        {
            path = Path.Combine(AppContext.BaseDirectory, "configuration", "endpoints.json");
        }

        var json = File.ReadAllText(path);
        var entries = JsonSerializer.Deserialize<JsonElement[]>(json)!;

        var items = new List<(EndpointInfo, int)>();

        foreach (var entry in entries)
        {
            var route = entry.GetProperty("route").GetString()!;
            var weight = entry.TryGetProperty("weight", out var w) ? w.GetInt32() : 1;

            var sampleParameters = new Dictionary<string, JsonElement[]>();
            if (entry.TryGetProperty("parameters", out var parametersElement))
            {
                foreach (var prop in parametersElement.EnumerateObject())
                {
                    sampleParameters[prop.Name] = prop.Value.EnumerateArray().ToArray();
                }
            }

            items.Add((new EndpointInfo(route, sampleParameters), weight));
        }

        return new WeightedList<EndpointInfo>(items);
    }

    private static Dictionary<string, object?> SelectParameters(
        Dictionary<string, JsonElement[]> sampleParameters)
    {
        var dict = new Dictionary<string, object?>();

        foreach (var (name, samples) in sampleParameters)
        {
            var selected = samples[Random.Shared.Next(samples.Length)];
            dict[name] = selected.ValueKind switch
            {
                JsonValueKind.Null => null,
                JsonValueKind.String => selected.GetString(),
                JsonValueKind.Number when selected.TryGetInt64(out var l) => l,
                JsonValueKind.Number => selected.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => selected.GetRawText()
            };
        }

        return dict;
    }

    [GeneratedRegex(@"\{(\w+)\}")]
    private static partial Regex PathPlaceholderRegex();

    [LoggerMessage(Level = LogLevel.Information, Message = "OpenAPI load generator is disabled")]
    private static partial void LogDisabled(ILogger logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "No endpoints found in endpoints.json")]
    private static partial void LogNoEndpoints(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Loaded {count} endpoints")]
    private static partial void LogEndpointsLoaded(ILogger logger, int count);

    [LoggerMessage(Level = LogLevel.Information, Message = "Sending GET {uri}")]
    private static partial void LogSendingRequest(ILogger logger, string uri);

    [LoggerMessage(Level = LogLevel.Information, Message = "GET {uri} completed in {elapsedMs}ms - OK")]
    private static partial void LogRequestSuccess(ILogger logger, string uri, long elapsedMs);

    [LoggerMessage(Level = LogLevel.Error, Message = "GET {uri} failed in {elapsedMs}ms - HTTP {statusCode}")]
    private static partial void LogHttpError(ILogger logger, string uri, long elapsedMs, int statusCode);

    [LoggerMessage(Level = LogLevel.Error, Message = "Error requesting {route}")]
    private static partial void LogRequestException(ILogger logger, Exception ex, string route);

    private sealed record EndpointInfo(
        string Route,
        Dictionary<string, JsonElement[]> SampleParameters
    );
}
