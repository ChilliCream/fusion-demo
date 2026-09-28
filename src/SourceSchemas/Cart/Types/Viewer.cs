using System.Security.Claims;
using Demo.Cart.Data;
using HotChocolate.Authorization;

namespace Demo.Cart.Types;

public class Viewer
{
    [Authorize]
    public async Task<Data.Cart> GetCartAsync(
        ClaimsPrincipal claimsPrincipal,
        CartContext context,
        CancellationToken cancellationToken)
        => await context.GetOrCreateCartAsync(claimsPrincipal, cancellationToken);
}