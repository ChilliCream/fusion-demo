public static class Env
{
    public static string Version => typeof(Env).Assembly.GetName().Version?.ToString() ?? "0.0.0";
}