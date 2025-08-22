namespace Demo.Payments.Properties;

public static class Env
{
    public const string PaymentsApi = "payments-api";
    
    public const string PaymentsDb = "payments-db";
    
    public static string Version => typeof(Env).Assembly.GetName().Version?.ToString() ?? "0.0.0";
}