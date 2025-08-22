namespace Demo.Products.Properties;

public static class Env
{
    public const string ProductsApi = "products-api";
    
    public const string ProductsDb = "products-db";
    
    public static string Version => typeof(Env).Assembly.GetName().Version?.ToString() ?? "0.0.0";
}