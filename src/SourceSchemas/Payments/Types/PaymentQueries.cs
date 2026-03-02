namespace Demo.Payments.Types;

[QueryType]
public static partial class PaymentQueries
{
    [NodeResolver]
    public static async Task<Payment?> GetPaymentByIdAsync(
        int id,
        IPaymentByIdDataLoader productById,
        CancellationToken cancellationToken)
        => await productById.LoadAsync(id, cancellationToken);
}
