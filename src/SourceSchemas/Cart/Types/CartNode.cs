namespace Demo.Cart.Types;

[ObjectType<Data.Cart>]
public static partial class CartNode
{
    public static DateTime GetCreatedAt([Parent] Data.Cart cart)
        => cart.CreatedAt;

    [UsePaging]
    public static IEnumerable<Data.CartItem> GetItems([Parent] Data.Cart cart)
        => cart.Items;
}
