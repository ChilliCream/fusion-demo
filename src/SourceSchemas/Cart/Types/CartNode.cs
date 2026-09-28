using Demo.Cart.Data;
using GreenDonut.Data;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;

namespace Demo.Cart.Types;

[ObjectType<Data.Cart>]
public static partial class CartNode
{
    [ID]
    public static int GetId([Parent] Data.Cart cart)
        => cart.Id;

    // OwnerId is the shopper's JWT subject and is used only to scope cart
    // lookups server-side; it must not be exposed on the public schema.
    static partial void Configure(IObjectTypeDescriptor<Data.Cart> descriptor)
        => descriptor.Ignore(c => c.OwnerId);

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
