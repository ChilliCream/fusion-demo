namespace Demo.Payments.Types;

[ExtendObjectType<Order>]
public static class OrderExtensions
{
    public static async Task<Payment[]> GetPaymentsAsync(
        [Parent] Order order,
        [Service] PaymentByOrderIdDataLoader dataLoader,
        CancellationToken cancellationToken)
    {
        var res = await dataLoader.LoadAsync(order.Id, cancellationToken);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (res is null)
        {
            return Array.Empty<Payment>();
        }

        return res;
    }
}
