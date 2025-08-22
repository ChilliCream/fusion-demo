namespace Demo.Order.Data;

internal static class OrderDataLoader
{
    [DataLoader]
    internal static async Task<Dictionary<int, Order>> GetOrderByIdAsync(
        IReadOnlyList<int> ids,
        OrderContext context,
        CancellationToken cancellationToken)
        => await context.Order
            .Where(t => ids.Contains(t.Id))
            .Include(t => t.Items)
            .ToDictionaryAsync(t => t.Id, cancellationToken);
    
    [DataLoader]
    internal static async Task<Dictionary<int, Order>> GetOrderByUserIdAsync(
        IReadOnlyList<int> userIds,
        OrderContext context,
        CancellationToken cancellationToken)
        => await context.Order
            .Where(t => userIds.Contains(t.UserId))
            .Include(t => t.Items)
            .ToDictionaryAsync(t => t.Id, cancellationToken);
}
