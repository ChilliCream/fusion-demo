namespace Demo.Cart.Properties;

public static class Env
{
    public const string CartApi = "cart-api";
    public const string CartDb = "cart-db";
    public static string Version => typeof(Env).Assembly.GetName().Version?.ToString() ?? "0.0.0";
}
