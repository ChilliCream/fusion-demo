using Demo.Cart.Data;
using Microsoft.EntityFrameworkCore;

namespace Demo.Cart.Types;

[MutationType]
public static partial class CartMutations
{
    public static async Task<Data.Cart> AddToCartAsync(
        [ID<Product>] int productId,
        CartContext context,
        CancellationToken cancellationToken)
    {
        var cart = await context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(cancellationToken);

        if (cart is null)
        {
            cart = new Data.Cart
            {
                CreatedAt = DateTime.UtcNow
            };
            context.Carts.Add(cart);
            await context.SaveChangesAsync(cancellationToken);
        }

        var cartItem = new CartItem
        {
            CartId = cart.Id,
            ProductId = productId,
            AddedAt = DateTime.UtcNow
        };

        cart.Items.Add(cartItem);
        await context.SaveChangesAsync(cancellationToken);

        return cart;
    }

    public static async Task<Data.Cart?> RemoveFromCartAsync(
        [ID<Product>] int productId,
        CartContext context,
        CancellationToken cancellationToken)
    {
        var cart = await context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(cancellationToken);

        if (cart is null)
        {
            return null;
        }

        var itemToRemove = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (itemToRemove is not null)
        {
            cart.Items.Remove(itemToRemove);
            context.CartItems.Remove(itemToRemove);
            await context.SaveChangesAsync(cancellationToken);
        }

        return cart;
    }

    public static async Task<Data.Cart?> CheckoutAsync(
        CartContext context,
        CancellationToken cancellationToken)
    {
        var cart = await context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(cancellationToken);

        if (cart is null)
        {
            return null;
        }

        context.CartItems.RemoveRange(cart.Items);
        cart.Items.Clear();
        await context.SaveChangesAsync(cancellationToken);

        return cart;
    }
}
