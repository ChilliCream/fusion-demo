namespace Demo.Inventory.Types;

[QueryType]
public static partial class Query
{
    [NodeResolver]
    public static async Task<InventoryItem?> GetInventoryItemByIdAsync(
        int id,
        IInventoryItemByIdDataLoader inventoryItemById,
        CancellationToken cancellationToken)
        => await inventoryItemById.LoadAsync(id, cancellationToken);
}
