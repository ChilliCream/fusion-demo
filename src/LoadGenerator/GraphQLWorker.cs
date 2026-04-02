using System.Diagnostics;
using System.Text.Json;
using HotChocolate.Language;
using HotChocolate.Transport;
using HotChocolate.Transport.Http;
using Microsoft.Extensions.Options;

namespace LoadGenerator;

public sealed partial class GraphQLWorker(
    IHttpClientFactory httpClientFactory,
    IOptions<LoadGeneratorOptions> options,
    ILogger<GraphQLWorker> logger,
    IHostEnvironment environment) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = options.Value;
        var workerConfig = config.GraphQL;

        if (!workerConfig.Enabled)
        {
            LogDisabled(logger);
            return;
        }

        var queries = LoadQueries();

        if (queries.Count == 0)
        {
            LogNoQueries(logger);
            return;
        }

        LogQueriesLoaded(logger, queries.Count);

        var httpClient = httpClientFactory.CreateClient("GraphQL");
        using var client = new DefaultGraphQLHttpClient(httpClient);
        using var semaphore = new SemaphoreSlim(workerConfig.MaxConcurrentRequests);

        while (!stoppingToken.IsCancellationRequested)
        {
            var batchSize = Random.Shared.Next(1, workerConfig.MaxRequestsPerBatch + 1);
            var batch = Enumerable.Range(0, batchSize)
                .Select(_ => queries.SelectRandom())
                .ToList();

            var tasks = new Task[batch.Count];
            for (var i = 0; i < batch.Count; i++)
            {
                tasks[i] = ExecuteQueryAsync(client, batch[i], semaphore, stoppingToken);
            }

            await Task.WhenAll(tasks);

            var delayMs = Random.Shared.Next(workerConfig.MinDelayMs, workerConfig.MaxDelayMs + 1);
            await Task.Delay(delayMs, stoppingToken);
        }
    }

    private async Task ExecuteQueryAsync(
        DefaultGraphQLHttpClient client,
        QueryInfo query,
        SemaphoreSlim semaphore,
        CancellationToken ct)
    {
        await semaphore.WaitAsync(ct);
        try
        {
            var variables = SelectVariables(query.SampleVariables);
            var variablesJson = variables.Count > 0 ? JsonSerializer.Serialize(variables) : "{}";

            LogSendingQuery(logger, query.Name, variablesJson);

            var request = options.Value.GraphQL.UsePersistedOperations
                ? new OperationRequest(
                    query: null,
                    id: query.Id,
                    operationName: null,
                    variables: variables.Count > 0 ? variables : null,
                    extensions: null)
                : new OperationRequest(
                    query: query.Body,
                    id: null,
                    operationName: null,
                    variables: variables.Count > 0 ? variables : null,
                    extensions: null);

            var sw = Stopwatch.StartNew();
            using var response = await client.PostAsync(request, new Uri("graphql", UriKind.Relative), ct);
            sw.Stop();

            if (response.IsSuccessStatusCode)
            {
                using var result = await response.ReadAsResultAsync(ct);
                var hasErrors = result.Errors.ValueKind is not JsonValueKind.Undefined
                    and not JsonValueKind.Null;

                if (hasErrors)
                {
                    LogGraphQLErrors(logger, query.Name, sw.ElapsedMilliseconds, result.Errors.ToString());
                }
                else
                {
                    LogQuerySuccess(logger, query.Name, sw.ElapsedMilliseconds);
                }
            }
            else
            {
                LogHttpError(logger, query.Name, sw.ElapsedMilliseconds, (int)response.StatusCode);
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // Shutting down
        }
        catch (Exception ex)
        {
            LogQueryException(logger, ex, query.Name);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private WeightedList<QueryInfo> LoadQueries()
    {
        var path = Path.Combine(environment.ContentRootPath, "configuration", "queries.json");

        if (!File.Exists(path))
        {
            path = Path.Combine(AppContext.BaseDirectory, "configuration", "queries.json");
        }

        var json = File.ReadAllText(path);
        var entries = JsonSerializer.Deserialize<JsonElement[]>(json)!;

        var items = new List<(QueryInfo, int)>();

        foreach (var entry in entries)
        {
            var id = entry.GetProperty("id").GetString()!;
            var body = entry.GetProperty("query").GetString()!;
            var weight = entry.TryGetProperty("weight", out var w) ? w.GetInt32() : 1;
            var document = Utf8GraphQLParser.Parse(body);
            var operation = document.Definitions.OfType<OperationDefinitionNode>().First();
            var name = operation.Name?.Value ?? id;

            var sampleVariables = new Dictionary<string, JsonElement[]>();
            if (entry.TryGetProperty("variables", out var variablesElement))
            {
                foreach (var prop in variablesElement.EnumerateObject())
                {
                    sampleVariables[prop.Name] = prop.Value.EnumerateArray().ToArray();
                }
            }

            items.Add((new QueryInfo(id, name, body, sampleVariables), weight));
        }

        return new WeightedList<QueryInfo>(items);
    }

    private static Dictionary<string, object?> SelectVariables(
        Dictionary<string, JsonElement[]> sampleVariables)
    {
        var dict = new Dictionary<string, object?>();

        foreach (var (name, samples) in sampleVariables)
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

    [LoggerMessage(Level = LogLevel.Information, Message = "Load generator is disabled")]
    private static partial void LogDisabled(ILogger logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "No queries found in queries.json")]
    private static partial void LogNoQueries(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Loaded {count} queries")]
    private static partial void LogQueriesLoaded(ILogger logger, int count);

    [LoggerMessage(Level = LogLevel.Information, Message = "Sending {query} with variables: {variables}")]
    private static partial void LogSendingQuery(ILogger logger, string query, string variables);

    [LoggerMessage(Level = LogLevel.Information, Message = "{query} completed in {elapsedMs}ms - OK")]
    private static partial void LogQuerySuccess(ILogger logger, string query, long elapsedMs);

    [LoggerMessage(Level = LogLevel.Warning, Message = "{query} completed in {elapsedMs}ms with GraphQL errors: {errors}")]
    private static partial void LogGraphQLErrors(ILogger logger, string query, long elapsedMs, string errors);

    [LoggerMessage(Level = LogLevel.Error, Message = "{query} failed in {elapsedMs}ms - HTTP {statusCode}")]
    private static partial void LogHttpError(ILogger logger, string query, long elapsedMs, int statusCode);

    [LoggerMessage(Level = LogLevel.Error, Message = "Error executing {query}")]
    private static partial void LogQueryException(ILogger logger, Exception ex, string query);

    private sealed record QueryInfo(
        string Id,
        string Name,
        string Body,
        Dictionary<string, JsonElement[]> SampleVariables
    );
}
