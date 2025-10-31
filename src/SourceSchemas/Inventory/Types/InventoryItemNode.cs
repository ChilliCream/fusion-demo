namespace Demo.Inventory.Types;

[ObjectType<InventoryItem>]
public static partial class InventoryItemNode
{
    [BindMember(nameof(InventoryItem.ProductId))]
    public static Product GetProduct([Parent] InventoryItem item)
        => new(item.ProductId, item);
}
