namespace Demo.Inventory.Types;

public sealed class Product(int id, InventoryItem? item = null)
{
    private InventoryItem? _item = item;

    [ID<Product>]
    public int Id { get; } = id;

    [Tag("team-inventor")]
    public async Task<InventoryItem?> GetItemAsync(
        IInventoryItemByProductIdDataLoader inventoryItemByProductId,
        CancellationToken cancellationToken)
    {
        _item ??= await inventoryItemByProductId.LoadAsync(Id, cancellationToken);
        return _item;  
    }

    [Tag("team-inventor")]
    public async Task<int> GetQuantityAsync(
        IInventoryItemByProductIdDataLoader inventoryItemByProductId,
        CancellationToken cancellationToken)
    {
        _item ??= await inventoryItemByProductId.LoadAsync(Id, cancellationToken);
        return _item?.Quantity ?? 0;
    }
}
