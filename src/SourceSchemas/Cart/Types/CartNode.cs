using Demo.Cart.Data;
using GreenDonut.Data;
using HotChocolate.Types.Pagination;

namespace Demo.Cart.Types;

[ObjectType<Data.Cart>]
public static partial class CartNode
{
    [ID]
    public static int GetId([Parent] Data.Cart cart)
        => cart.Id;
    
    [UsePaging(ConnectionName = "CartItems")]
    public static async Task<Connection<CartItem>> GetItemsAsync(
        [Parent] Data.Cart cart,
        PagingArguments pagingArgs,
        CartContext context,
        CancellationToken cancellationToken)
    {
        return await context.CartItems
            .Where(item => item.CartId == cart.Id)
            .OrderBy(item => item.AddedAt)
            .ToPageAsync(pagingArgs, cancellationToken)
            .ToConnectionAsync();
    }
}
