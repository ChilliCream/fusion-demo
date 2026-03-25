namespace Demo.Inventory.Data;

public class InventoryContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<InventoryItem> Inventory => Set<InventoryItem>();
    
    public static async Task SeedDataAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<InventoryContext>();

        await context.Database.MigrateAsync(cancellationToken);
    }
}
