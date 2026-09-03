using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;

namespace Demo.Cart.Data;

internal static class CartDataLoader
{
    [DataLoader]
    public static async Task<Dictionary<int, Cart>> GetCartByIdAsync(
        IReadOnlyList<int> ids,
        CartContext context,
        CancellationToken cancellationToken)
        => await context.Carts
            .Where(cart => ids.Contains(cart.Id))
            .ToDictionaryAsync(cart => cart.Id, cancellationToken);

    [DataLoader]
    public static async Task<Dictionary<int, CartItem>> GetCartItemByIdAsync(
        IReadOnlyList<int> ids,
        QueryContext<CartItem> query,
        CartContext context,
        CancellationToken cancellationToken)
        => await context.CartItems
            .AsNoTracking()
            .Where(item => ids.Contains(item.Id))
            // Quantity is pinned alongside the key: GetLineTotal reads it off
            // the [Parent] entity directly rather than through a projected
            // GraphQL selection, so the projection middleware can't infer it.
            .With(query.Include(item => item.Id).Include(item => item.Quantity))
            .ToDictionaryAsync(item => item.Id, cancellationToken);
}
