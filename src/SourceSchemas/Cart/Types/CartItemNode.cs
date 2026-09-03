namespace Demo.Cart.Types;

[ObjectType<Data.CartItem>]
public static partial class CartItemNode
{
    static partial void Configure(IObjectTypeDescriptor<Data.CartItem> descriptor)
    {
        descriptor.Ignore(x => x.CartId);
    }

    [ID]
    public static int GetId([Parent] Data.CartItem cartItem)
        => cartItem.Id;

    public static DateTime GetAddedAt([Parent] Data.CartItem cartItem)
        => cartItem.AddedAt;

    [BindMember(nameof(Data.CartItem.ProductId))]
    public static Product GetProduct([Parent] Data.CartItem cartItem)
        => new Product(cartItem.ProductId);

    public static double GetUnitPrice(
        [Require("product.discountedPrice")] double price)
        => price;

    public static double GetLineTotal(
        [Parent] Data.CartItem cartItem,
        [Require("product.discountedPrice")] double price)
        => (double)Math.Round((decimal)price * cartItem.Quantity, 2, MidpointRounding.AwayFromZero);
}
