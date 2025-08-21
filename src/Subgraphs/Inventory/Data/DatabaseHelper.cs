namespace Demo.Inventory.Data;

public static class DatabaseHelper
{
    public static async Task SeedDatabaseAsync(WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<InventoryContext>();

        if (await context.Database.EnsureCreatedAsync())
        {
            await context.Inventory.AddRangeAsync(
                new InventoryItem { ProductId = 1, Quantity = 10 },
                new InventoryItem { ProductId = 2, Quantity = 5 },
                new InventoryItem { ProductId = 3, Quantity = 20 });
            await context.SaveChangesAsync();
        }
    }
}
