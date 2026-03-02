namespace Demo.Inventory.Types;

[MutationType]
public static partial class Mutation
{
    public static async Task<int> RestockProductAsync(
        int productId,
        int quantity,
        [Service] InventoryContext context)
    {
        var inventory = await context.Inventory.FindAsync(productId);

        if (inventory is null)
        {
            inventory = new InventoryItem { ProductId = productId, Quantity = quantity };
            context.Inventory.Add(inventory);
        }

        inventory.Quantity += quantity;

        await context.SaveChangesAsync();

        return productId;
    }
}
