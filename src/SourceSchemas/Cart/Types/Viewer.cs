using Demo.Cart.Data;
using Microsoft.EntityFrameworkCore;

namespace Demo.Cart.Types;

public class Viewer
{
    public async Task<Data.Cart?> GetCartAsync(
        CartContext context,
        CancellationToken cancellationToken)
    {
        return await context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(cancellationToken);
    }
}