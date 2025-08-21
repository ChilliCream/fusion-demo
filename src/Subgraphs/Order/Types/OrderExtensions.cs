namespace Demo.Order.Types;

[ExtendObjectType<Data.Order>]
public static class OrderExtensions
{
    [BindMember(nameof(Data.Order.UserId))]
    public static User GetUser([Parent] Data.Order order) => new(order.UserId);
}
