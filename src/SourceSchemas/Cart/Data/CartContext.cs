using Microsoft.EntityFrameworkCore;

namespace Demo.Cart.Data;

public class CartContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Cart> Carts => Set<Cart>();
    
    public DbSet<CartItem> CartItems => Set<CartItem>();

    public static async Task SeedDataAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<CartContext>();

        await context.Database.MigrateAsync(cancellationToken);
    }
}
