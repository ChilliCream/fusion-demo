public sealed class Configuration
{
    public string Version { get; } = typeof(Configuration).Assembly.GetName().Version?.ToString() ?? "0.0.0";
}