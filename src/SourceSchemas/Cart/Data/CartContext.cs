using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Demo.Cart.Data;

public class CartContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Cart> Carts => Set<Cart>();

    public DbSet<CartItem> CartItems => Set<CartItem>();

    /// <summary>
    /// Finds the signed-in shopper's cart, or <see langword="null"/> if they do not have one yet.
    /// </summary>
    public Task<Cart?> FindCartAsync(
        ClaimsPrincipal owner,
        CancellationToken cancellationToken)
    {
        var ownerId = GetOwnerId(owner);

        return Carts.FirstOrDefaultAsync(c => c.OwnerId == ownerId, cancellationToken);
    }

    /// <summary>
    /// Finds the signed-in shopper's cart, creating it if this is their first one.
    /// </summary>
    public async Task<Cart> GetOrCreateCartAsync(
        ClaimsPrincipal owner,
        CancellationToken cancellationToken)
    {
        var ownerId = GetOwnerId(owner);

        var cart = await Carts.FirstOrDefaultAsync(c => c.OwnerId == ownerId, cancellationToken);

        if (cart is null)
        {
            cart = new Cart
            {
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow
            };

            Carts.Add(cart);
            await SaveChangesAsync(cancellationToken);
        }

        return cart;
    }

    private static string GetOwnerId(ClaimsPrincipal owner)
    {
        var ownerId = owner.FindFirst("sub")?.Value
            ?? owner.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(ownerId))
        {
            throw new InvalidOperationException(
                "The authenticated request is missing a subject claim.");
        }

        return ownerId;
    }

    public static async Task SeedDataAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<CartContext>();

        await context.Database.MigrateAsync(cancellationToken);
    }
}
