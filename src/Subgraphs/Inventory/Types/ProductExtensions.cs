namespace Demo.Inventory.Types;

[ExtendObjectType<InventoryItem>]
public static class ProductExtensions
{
    [BindMember(nameof(InventoryItem.ProductId))]
    public static Product GetProduct([Parent] InventoryItem item)
        => new(item.ProductId, item.Quantity);
}
