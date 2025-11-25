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
                new InventoryItem { ProductId = 1, Quantity = 10 },    // Table
                new InventoryItem { ProductId = 2, Quantity = 5 },     // Couch
                new InventoryItem { ProductId = 3, Quantity = 20 },    // Chair
                new InventoryItem { ProductId = 4, Quantity = 15 },    // Bookshelf
                new InventoryItem { ProductId = 5, Quantity = 8 },     // Desk
                new InventoryItem { ProductId = 6, Quantity = 12 },    // Bed Frame
                new InventoryItem { ProductId = 7, Quantity = 25 },    // Nightstand
                new InventoryItem { ProductId = 8, Quantity = 18 },    // Coffee Table
                new InventoryItem { ProductId = 9, Quantity = 30 },    // Dining Chair
                new InventoryItem { ProductId = 10, Quantity = 0 },    // Wardrobe - OUT OF STOCK
                new InventoryItem { ProductId = 11, Quantity = 14 },   // TV Stand
                new InventoryItem { ProductId = 12, Quantity = 9 },    // Dresser
                new InventoryItem { ProductId = 13, Quantity = 22 },   // Armchair
                new InventoryItem { ProductId = 14, Quantity = 35 },   // Bar Stool
                new InventoryItem { ProductId = 15, Quantity = 0 },    // Sideboard - OUT OF STOCK
                new InventoryItem { ProductId = 16, Quantity = 28 },   // Bench
                new InventoryItem { ProductId = 17, Quantity = 11 },   // Rocking Chair
                new InventoryItem { ProductId = 18, Quantity = 16 }    // Storage Cabinet
            );

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
