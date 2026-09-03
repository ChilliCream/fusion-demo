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
}
