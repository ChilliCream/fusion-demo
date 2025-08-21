namespace Demo.Payments.Types;

[ExtendObjectType<Payment>]
public static class PaymentNode
{
    [DataLoader]
    internal static async Task<IReadOnlyDictionary<int, Payment[]>> GetPaymentByOrderIdAsync(
        IReadOnlyList<int> ids,
        PaymentContext context,
        CancellationToken cancellationToken)
        => await context.Payments
            .Where(t => ids.Contains(t.OrderId))
            .GroupBy(t => t.OrderId)
            .ToDictionaryAsync(t => t.Key, t => t.ToArray(), cancellationToken);

    [DataLoader]
    internal static async Task<IReadOnlyDictionary<int, Payment>> GetPaymentByIdAsync(
        IReadOnlyList<int> ids,
        PaymentContext context,
        CancellationToken cancellationToken)
        => await context.Payments
            .Where(t => ids.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, cancellationToken);
}
