namespace Demo.Payments.Properties;

public static class Env
{
    public const string PaymentApi = "payment-api";
    
    public const string PaymentDb = "payment-db";
    
    public static string Version => typeof(Env).Assembly.GetName().Version?.ToString() ?? "0.0.0";
}