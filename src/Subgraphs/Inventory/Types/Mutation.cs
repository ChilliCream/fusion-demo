namespace Demo.Inventory.Types;

[MutationType]
public static class Mutation
{
    public static async Task<Product> RestockProductAsync(
        [ID<Product>] int id,
        int quantity,
        [Service] InventoryContext context)
    {
        var product = await context.Inventory.FindAsync(id);

        if (product is null)
        {
            product = new InventoryItem { ProductId = id, Quantity = quantity };
            context.Inventory.Add(product);
        }

        product.Quantity += quantity;

        await context.SaveChangesAsync();

        return new Product(product.Id, product.Quantity);
    }
}
