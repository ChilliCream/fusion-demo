namespace LoadGenerator.Properties;

public static class Env
{
    public const string LoadGenerator = "load-generator";

    public static string Version => typeof(Env).Assembly.GetName().Version?.ToString() ?? "0.0.0";
}
