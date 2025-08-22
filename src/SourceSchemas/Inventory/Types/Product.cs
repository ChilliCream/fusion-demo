namespace Demo.Inventory.Types;

public record Product([property: ID<Product>] int Id, InventoryItem? Item = null)
{
    public async Task<int> GetQuantityAsync(
        InventoryItemByProductIdDataLoader inventoryItemByProductId,
        CancellationToken cancellationToken)
    {
        if (Item is not null)
        {
            return Item.Quantity;
        }
        
        var item = await inventoryItemByProductId.LoadAsync(Id, cancellationToken);
        return item?.Quantity ?? 0;
    }
}
