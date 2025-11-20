using Demo.Cart.Data;
using Microsoft.EntityFrameworkCore;

namespace Demo.Cart.Types;

public class Viewer
{
    public async Task<Data.Cart> GetCartAsync(
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

        return cart;
    }
}