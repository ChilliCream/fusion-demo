namespace Demo.Inventory.Types;

[ExtendObjectType<InventoryItem>]
public static class InventoryItemNode
{
    [DataLoader]
    internal static async Task<IReadOnlyDictionary<int, InventoryItem>>
        GetInventoryItemByProductIdAsync(
        IReadOnlyList<int> ids,
        InventoryContext context,
        CancellationToken cancellationToken)
        => await context.Inventory
            .Where(t => ids.Contains(t.ProductId))
            .ToDictionaryAsync(t => t.Id, cancellationToken);

    [DataLoader]
    internal static async Task<IReadOnlyDictionary<int, InventoryItem>>
        GetInventoryItemByIdAsync(
        IReadOnlyList<int> ids,
        InventoryContext context,
        CancellationToken cancellationToken)
        => await context.Inventory
            .Where(t => ids.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, cancellationToken);
}
