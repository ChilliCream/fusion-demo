namespace Demo.Order.Properties;

public static class Env
{
    public const string OrderApi = "order-api";
    
    public const string OrderDb = "order-db";
    
    public static string Version => typeof(Env).Assembly.GetName().Version?.ToString() ?? "0.0.0";
}