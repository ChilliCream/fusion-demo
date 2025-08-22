namespace Demo.Order.Types;

[MutationType]
public static partial class OrderMutations
{
    public static async Task<Data.Order> CreateOrderAsync(
        [ID<User>] int userId,
        OrderItemInput[] items,
        int weight,
        [Service] OrderContext context,
        CancellationToken cancellationToken)
    {
        var order = new Data.Order
        {
            UserId = userId, Weight = weight, Items = new List<OrderItem>()
        };

        foreach (var item in items)
        {
            order.Items.Add(new OrderItem
            {
                ProductId = item.ProductId, Quantity = item.Quantity, Price = item.Price
            });
        }

        context.Order.Add(order);
        await context.SaveChangesAsync(cancellationToken);

        return order;
    }
}