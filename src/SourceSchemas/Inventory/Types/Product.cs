namespace Demo.Inventory.Types;

public sealed class Product(int id, InventoryItem? item = null)
{
    [ID<Product>]
    public int Id { get; } = id;

    public async Task<InventoryItem?> GetItemAsync(
        InventoryItemByProductIdDataLoader inventoryItemByProductId,
        CancellationToken cancellationToken)
    {
        item ??= await inventoryItemByProductId.LoadAsync(Id, cancellationToken);
        return item;  
    }

    public async Task<int> GetQuantityAsync(
        InventoryItemByProductIdDataLoader inventoryItemByProductId,
        CancellationToken cancellationToken)
    {
        item ??= await inventoryItemByProductId.LoadAsync(Id, cancellationToken);
        return item?.Quantity ?? 0;
    }
}
