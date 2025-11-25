using Demo.Cart.Data;
using Microsoft.EntityFrameworkCore;

namespace Demo.Cart.Types;

[MutationType]
public static partial class CartMutations
{
    [Error<ProductAmountCannotBeLowerThanOneException>]
    public static async Task<Data.Cart> AddProductToCartAsync(
        [ID<Product>] int productId,
        int quantity,
        CartContext context,
        CancellationToken cancellationToken)
    {
        if (quantity < 1)
        {
            throw new ProductAmountCannotBeLowerThanOneException(productId, quantity);
        }

        var cart = await context.Carts.FirstOrDefaultAsync(cancellationToken);

        if (cart is null)
        {
            cart = new Data.Cart
            {
                CreatedAt = DateTime.UtcNow
            };
            context.Carts.Add(cart);
            await context.SaveChangesAsync(cancellationToken);
        }

        var existingCartItem = await context.CartItems.FirstOrDefaultAsync(
            item => item.CartId == cart.Id
                && item.ProductId == productId, cancellationToken);

        if (existingCartItem is not null)
        {
            existingCartItem.Quantity += quantity;
        }
        else
        {
            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = productId,
                Quantity = quantity,
                AddedAt = DateTime.UtcNow
            };
            context.CartItems.Add(cartItem);
        }

        await context.SaveChangesAsync(cancellationToken);

        return cart;
    }

    [Error<ProductAmountCannotBeLowerThanOneException>]
    public static async Task<Data.Cart?> RemoveProductFromCartAsync(
        [ID<Product>] int productId,
        int quantity,
        CartContext context,
        CancellationToken cancellationToken)
    {
        if (quantity < 1)
        {
            throw new ProductAmountCannotBeLowerThanOneException(productId, quantity);
        }

        var cart = await context.Carts
            .FirstOrDefaultAsync(cancellationToken);

        if (cart is null)
        {
            return null;
        }

        var cartItem = await context.CartItems.FirstOrDefaultAsync(
            item => item.CartId == cart.Id
                && item.ProductId == productId, cancellationToken);

        if (cartItem is not null)
        {
            cartItem.Quantity -= quantity;

            if (cartItem.Quantity <= 0)
            {
                context.CartItems.Remove(cartItem);
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        return cart;
    }

    public static async Task<Data.Cart?> CheckoutAsync(
        CartContext context,
        CancellationToken cancellationToken)
    {
        var cart = await context.Carts.FirstOrDefaultAsync(cancellationToken);

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
