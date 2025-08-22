namespace Demo.Payments.Types;

[QueryType]
public static partial class PaymentQueries
{
    [Lookup, NodeResolver]
    public static async Task<Payment?> GetPaymentByIdAsync(
        int id,
        PaymentByIdDataLoader productById,
        CancellationToken cancellationToken)
        => await productById.LoadAsync(id, cancellationToken);

    [Lookup, Internal]
    public static Order GetOrderById([ID<Order>] int id) 
        => new(id);
}
