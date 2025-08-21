namespace Demo.Inventory.Data;

public class InventoryContext : DbContext
{
    public InventoryContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<InventoryItem> Inventory => Set<InventoryItem>();
}
