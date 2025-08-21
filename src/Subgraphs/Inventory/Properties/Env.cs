public static class Env
{
    public const string InventoryApi = "inventory-api";
    
    public const string InventoryDb = "inventory-db";
    
    public static string Version => typeof(Env).Assembly.GetName().Version?.ToString() ?? "0.0.0";
}