namespace Demo.Shipping.Properties;

public static class Env
{
    public const string ShippingApi = "shipping-api";
    
    public static string Version => typeof(Env).Assembly.GetName().Version?.ToString() ?? "0.0.0";
}