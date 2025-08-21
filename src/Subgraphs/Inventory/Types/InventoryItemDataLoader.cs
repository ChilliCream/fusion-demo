namespace Demo.Inventory.Types;

public static class InventoryItemDataLoader
{
    [DataLoader]
    internal static async Task<Dictionary<int, InventoryItem>> GetInventoryItemByProductIdAsync(
        IReadOnlyList<int> ids,
        InventoryContext context,
        CancellationToken cancellationToken)
        => await context.Inventory
            .Where(t => ids.Contains(t.ProductId))
            .ToDictionaryAsync(t => t.Id, cancellationToken);

    [DataLoader]
    internal static async Task<Dictionary<int, InventoryItem>> GetInventoryItemByIdAsync(
        IReadOnlyList<int> ids,
        InventoryContext context,
        CancellationToken cancellationToken)
        => await context.Inventory
            .Where(t => ids.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, cancellationToken);
}
