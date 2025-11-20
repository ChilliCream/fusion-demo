using Demo.Cart.Data;
using Microsoft.EntityFrameworkCore;

namespace Demo.Cart.Types;

[MutationType]
public static partial class CartMutations
{
    public static async Task<Data.Cart> AddProductToCartAsync(
        [ID<Product>] int productId,
        CartContext context,
        CancellationToken cancellationToken)
    {
        var cart = await context.Carts
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

        context.CartItems.Add(cartItem);
        await context.SaveChangesAsync(cancellationToken);

        return cart;
    }

    public static async Task<Data.Cart?> RemoveProductFromCartAsync(
        [ID<Product>] int productId,
        CartContext context,
        CancellationToken cancellationToken)
    {
        var cart = await context.Carts
            .FirstOrDefaultAsync(cancellationToken);

        if (cart is null)
        {
            return null;
        }

        var itemToRemove = await context.CartItems
            .FirstOrDefaultAsync(i => i.CartId == cart.Id && i.ProductId == productId, cancellationToken);

        if (itemToRemove is not null)
        {
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
            .FirstOrDefaultAsync(cancellationToken);

        if (cart is null)
        {
            return null;
        }

        var cartItems = await context.CartItems
            .Where(i => i.CartId == cart.Id)
            .ToListAsync(cancellationToken);

        context.CartItems.RemoveRange(cartItems);
        await context.SaveChangesAsync(cancellationToken);

        return cart;
    }
}
