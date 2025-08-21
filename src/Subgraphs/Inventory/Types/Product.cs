namespace Demo.Inventory.Types;

public record Product([property: ID<Product>] int Id)
{
    public async Task<int> GetQuantityAsync(
        InventoryItemByProductIdDataLoader inventoryItemByProductId,
        CancellationToken cancellationToken)
    {
        var item = await inventoryItemByProductId.LoadAsync(Id, cancellationToken);
        return item?.Quantity ?? 0;
    }
}
