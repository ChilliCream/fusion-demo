using HotChocolate.Execution;
using Microsoft.EntityFrameworkCore;

namespace Demo.Cart.Data;

public class CartContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Cart> Carts => Set<Cart>();
    
    public DbSet<CartItem> CartItems => Set<CartItem>();

    public static async Task SeedDataAsync(
        IRequestExecutor executor,
        CancellationToken cancellationToken = default)
    {
        var services = executor.Schema.Services.GetRootServiceProvider();
        await using var scope = services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<CartContext>();

        // Ensure database is created
        await context.Database.EnsureCreatedAsync(cancellationToken);
    }
}
