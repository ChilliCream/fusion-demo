using HotChocolate.Execution;

namespace Demo.Order.Data;

public class OrderContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Order> Order => Set<Order>();
    
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    
    public static async Task SeedDataAsync(
        IRequestExecutor executor, 
        CancellationToken cancellationToken = default)
    {
        var services = executor.Schema.Services.GetRootServiceProvider();

        await using var scope = services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<OrderContext>();

        if (await context.Database.EnsureCreatedAsync(cancellationToken))
        {
            await context.Order.AddRangeAsync(
                new Order
                {
                    UserId = 1,
                    Weight = 100,
                    Items =
                    [
                        new OrderItem { Price = 899.99, Quantity = 1, ProductId = 1 },
                        new OrderItem { Price = 54, Quantity = 1, ProductId = 3 }
                    ]
                },
                new Order
                {
                    UserId = 2,
                    Weight = 1000,
                    Items =
                    [
                        new OrderItem { Price = 899.99, Quantity = 1, ProductId = 1 },
                        new OrderItem { Price = 1299.50, Quantity = 1, ProductId = 2 }
                    ]
                },
                new Order
                {
                    UserId = 3,
                    Weight = 50,
                    Items =
                    [
                        new OrderItem { Price = 899.99, Quantity = 1, ProductId = 1 },
                        new OrderItem { Price = 54, Quantity = 1, ProductId = 3 }
                    ]
                });
            
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
