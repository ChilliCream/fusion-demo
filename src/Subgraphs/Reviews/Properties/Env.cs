public static class Env
{
    public const string ReviewsApi = "reviews-api";
    
    public const string ReviewsDb = "reviews-db";
    
    public static string Version => typeof(Env).Assembly.GetName().Version?.ToString() ?? "0.0.0";
}