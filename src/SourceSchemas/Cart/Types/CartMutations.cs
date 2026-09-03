using System.Security.Claims;
using Demo.Cart.Data;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Demo.Cart.Types;

[MutationType]
public static partial class CartMutations
{
    [Authorize]
    [Error<ProductAmountCannotBeLowerThanOneException>]
    public static async Task<Data.Cart> AddProductToCartAsync(
        [ID<Product>] int productId,
        int quantity,
        ClaimsPrincipal claimsPrincipal,
        CartContext context,
        CancellationToken cancellationToken)
    {
        if (quantity < 1)
        {
            throw new ProductAmountCannotBeLowerThanOneException(productId, quantity);
        }

        var cart = await context.GetOrCreateCartAsync(claimsPrincipal, cancellationToken);

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

    [Authorize]
    [Error<ProductAmountCannotBeLowerThanOneException>]
    public static async Task<Data.Cart?> RemoveProductFromCartAsync(
        [ID<Product>] int productId,
        int quantity,
        ClaimsPrincipal claimsPrincipal,
        CartContext context,
        CancellationToken cancellationToken)
    {
        if (quantity < 1)
        {
            throw new ProductAmountCannotBeLowerThanOneException(productId, quantity);
        }

        var cart = await context.FindCartAsync(claimsPrincipal, cancellationToken);

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

    [Authorize]
    [Error<CartIsEmptyException>]
    public static async Task<Data.Cart> CheckoutAsync(
        ClaimsPrincipal claimsPrincipal,
        CartContext context,
        CancellationToken cancellationToken)
    {
        var cart = await context.GetOrCreateCartAsync(claimsPrincipal, cancellationToken);

        var cartItems = await context.CartItems
            .Where(i => i.CartId == cart.Id)
            .ToListAsync(cancellationToken);

        if (cartItems.Count == 0)
        {
            throw new CartIsEmptyException();
        }

        context.CartItems.RemoveRange(cartItems);
        context.Carts.Remove(cart);
        await context.SaveChangesAsync(cancellationToken);

        return await context.GetOrCreateCartAsync(claimsPrincipal, cancellationToken);
    }
}
