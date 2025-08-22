namespace Demo.Products.Properties;

public static class Env
{
    public const string ProductApi = "product-api";
    
    public const string ProductDb = "product-db";
    
    public static string Version => typeof(Env).Assembly.GetName().Version?.ToString() ?? "0.0.0";
}