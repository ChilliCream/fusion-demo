namespace Demo.Order.Types;

[ObjectType<OrderItem>]
public static partial class OrderItemNode
{
    [BindMember(nameof(OrderItem.ProductId))]
    public static Product GetProduct(
        [Parent] OrderItem orderItem) 
        => new(orderItem.ProductId);
}
