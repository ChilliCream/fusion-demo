namespace Demo.Cart.Types;

[ObjectType<Data.CartItem>]
public static partial class CartItemNode
{
    static partial void Configure(IObjectTypeDescriptor<Data.CartItem> descriptor)
    {
        descriptor.Ignore(x => x.CartId);
    }
    
    public static DateTime GetAddedAt([Parent] Data.CartItem cartItem)
        => cartItem.AddedAt;

    [BindMember(nameof(Data.CartItem.ProductId))]
    public static Product GetProduct([Parent] Data.CartItem cartItem)
        => new Product(cartItem.ProductId);
}
