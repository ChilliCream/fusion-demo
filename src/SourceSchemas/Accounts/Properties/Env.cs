namespace Demo.Accounts.Properties;

public static class Env
{
    public const string AccountsApi = "accounts-api";
    
    public const string AccountsDb = "accounts-db";
    
    public static string Version => typeof(Env).Assembly.GetName().Version?.ToString() ?? "0.0.0";
}