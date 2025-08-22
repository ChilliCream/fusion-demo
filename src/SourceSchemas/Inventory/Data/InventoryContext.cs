using HotChocolate.Execution;

namespace Demo.Inventory.Data;

public class InventoryContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<InventoryItem> Inventory => Set<InventoryItem>();
    
    public static async Task SeedDataAsync(
        IRequestExecutor executor, 
        CancellationToken cancellationToken = default)
    {
        var services = executor.Schema.Services.GetRootServiceProvider();

        await using var scope = services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<InventoryContext>();

        if (await context.Database.EnsureCreatedAsync(cancellationToken))
        {
            await context.Inventory.AddRangeAsync(
                new InventoryItem { ProductId = 1, Quantity = 10 },
                new InventoryItem { ProductId = 2, Quantity = 5 },
                new InventoryItem { ProductId = 3, Quantity = 20 });

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
