namespace Demo.Inventory.Data;

public class InventoryContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<InventoryItem> Inventory => Set<InventoryItem>();
}
