namespace Demo.Accounts.Properties;

public static class Env
{
    public const string AccountApi = "account-api";
    
    public static string Version => typeof(Env).Assembly.GetName().Version?.ToString() ?? "0.0.0";
}