namespace Demo.Order.Types;

[ExtendObjectType<OrderItem>]
public static class OrderItemExtensions
{
    [BindMember(nameof(OrderItem.ProductId))]
    public static Product GetProduct([Parent] OrderItem orderItem) => new(orderItem.ProductId);
}
