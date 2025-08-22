namespace Demo.Inventory.Types;

[MutationType]
public static partial class Mutation
{
    public static async Task<Product> RestockProductAsync(
        [ID<Product>] int id,
        int quantity,
        [Service] InventoryContext context)
    {
        var inventory = await context.Inventory.FindAsync(id);

        if (inventory is null)
        {
            inventory = new InventoryItem { ProductId = id, Quantity = quantity };
            context.Inventory.Add(inventory);
        }

        inventory.Quantity += quantity;

        await context.SaveChangesAsync();

        return new Product(id, inventory);
    }
}
