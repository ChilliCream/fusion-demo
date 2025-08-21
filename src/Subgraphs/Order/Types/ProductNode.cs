namespace Demo.Order.Types;

[ExtendObjectType<Data.Order>]
public static class OrderNode
{
    [DataLoader]
    internal static async Task<IReadOnlyDictionary<int, Data.Order>> GetOrderByIdAsync(
        IReadOnlyList<int> ids,
        OrderContext context,
        CancellationToken cancellationToken)
        => await context.Order
            .Where(t => ids.Contains(t.Id))
            .Include(t => t.Items)
            .ToDictionaryAsync(t => t.Id, cancellationToken);
}
