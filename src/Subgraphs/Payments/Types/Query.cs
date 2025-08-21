namespace Demo.Payments.Types;

[QueryType]
public static class Query
{
    [NodeResolver]
    public static async Task<Payment?> GetPaymentByIdAsync(
        int id,
        PaymentByIdDataLoader productById,
        CancellationToken cancellationToken)
        => await productById.LoadAsync(id, cancellationToken);

    public static Order GetOrderById([ID<Order>] int id)
    {
        return new Order(id);
    }

    public static IEnumerable<Order> GetOrdersByIds([ID<Order>] IEnumerable<int> ids)
    {
        return ids.Select(id => new Order(id));
    }
}
