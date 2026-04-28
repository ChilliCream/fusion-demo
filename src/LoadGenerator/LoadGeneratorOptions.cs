namespace LoadGenerator;

public sealed class LoadGeneratorOptions
{
    public const string SectionName = "LoadGenerator";

    public string GatewayUrl { get; set; } = "http://localhost:5000";
    public GraphQLWorkerOptions GraphQL { get; set; } = new();
    public WorkerOptions McpTools { get; set; } = new();
    public WorkerOptions McpPrompts { get; set; } = new();
    public WorkerOptions OpenApi { get; set; } = new();
}

public class WorkerOptions
{
    public int MinDelayMs { get; set; } = 500;
    public int MaxDelayMs { get; set; } = 3000;
    public int MaxConcurrentRequests { get; set; } = 5;
    public int MaxRequestsPerBatch { get; set; } = 10;
    public bool Enabled { get; set; } = true;
}

public sealed class GraphQLWorkerOptions : WorkerOptions
{
    public bool UsePersistedOperations { get; set; } = true;
    public string? ClientId { get; set; }
    public string? ClientVersion { get; set; }
}
