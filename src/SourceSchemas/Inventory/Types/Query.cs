namespace Demo.Inventory.Types;

[QueryType]
public static partial class Query
{
    [Lookup, NodeResolver]
    public static async Task<InventoryItem?> GetInventoryItemByIdAsync(
        int id,
        InventoryItemByIdDataLoader inventoryItemById,
        CancellationToken cancellationToken)
        => await inventoryItemById.LoadAsync(id, cancellationToken);

    [Lookup, Internal]
    public static Product? GetProductByIdAsync(
        [ID<Product>] int id,
        CancellationToken cancellationToken)
        => new(id);
}
