namespace Demo.Order.Types;

[ObjectType<Data.Order>]
public static partial class OrderNode
{
    [BindMember(nameof(Data.Order.UserId))]
    public static User GetUser([Parent] Data.Order order) 
        => new(order.UserId);
}
