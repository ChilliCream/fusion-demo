namespace Demo.Order.Data;

public class OrderContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Order> Order => Set<Order>();
    
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    
    public static async Task SeedDataAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<OrderContext>();

        await context.Database.MigrateAsync(cancellationToken);
    }
}
