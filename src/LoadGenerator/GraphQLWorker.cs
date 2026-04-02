using HotChocolate.Transport;
using HotChocolate.Transport.Http;
using Microsoft.Extensions.Options;
using SharpYaml;

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

            using var response = await client.PostAsync(request, new Uri("graphql", UriKind.Relative), ct);

            if (response.IsSuccessStatusCode)
            {
                await response.ReadAsResultAsync(ct);
            }
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

    private WeightedList<QueryInfo> LoadQueries()
    {
        var path = Path.Combine(environment.ContentRootPath, "configuration", "queries.yaml");

        if (!File.Exists(path))
        {
            path = Path.Combine(AppContext.BaseDirectory, "configuration", "queries.yaml");
        }

        var yaml = File.ReadAllText(path);
        var entries = YamlSerializer.Deserialize<List<Dictionary<string, object?>>>(yaml)!;

        var items = new List<(QueryInfo, int)>();

        foreach (var entry in entries)
        {
            var id = (string)entry["id"]!;
            var body = (string)entry["query"]!;
            var weight = entry.TryGetValue("weight", out var w) ? Convert.ToInt32(w) : 1;
            var sampleVariables = new Dictionary<string, object?[]>();
            if (entry.TryGetValue("variables", out var variablesObj)
                && variablesObj is Dictionary<string, object?> variables)
            {
                foreach (var (key, value) in variables)
                {
                    sampleVariables[key] = ((List<object?>) value!).ToArray();
                }
            }

            items.Add((new QueryInfo(id, body, sampleVariables), weight));
        }

        return new WeightedList<QueryInfo>(items);
    }

    private static Dictionary<string, object?> SelectVariables(
        Dictionary<string, object?[]> sampleVariables)
    {
        var dict = new Dictionary<string, object?>();

        foreach (var (name, samples) in sampleVariables)
        {
            dict[name] = samples[Random.Shared.Next(samples.Length)];
        }

        return dict;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "GraphQL load generator is disabled")]
    private static partial void LogDisabled(ILogger logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "No queries found in queries.yaml")]
    private static partial void LogNoQueries(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Loaded {count} queries")]
    private static partial void LogQueriesLoaded(ILogger logger, int count);

    private sealed record QueryInfo(
        string Id,
        string Body,
        Dictionary<string, object?[]> SampleVariables
    );
}
