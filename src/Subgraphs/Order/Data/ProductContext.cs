namespace Demo.Order.Data;

public class OrderContext : DbContext
{
    public OrderContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Order> Order => Set<Order>();
}
