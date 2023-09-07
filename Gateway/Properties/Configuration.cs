public static class App
{
    public static string Version { get; } = typeof(App).Assembly.GetName().Version?.ToString() ?? "0.0.0";
}