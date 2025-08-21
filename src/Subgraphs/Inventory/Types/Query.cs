using HotChocolate.Fusion.SourceSchema.Types;

namespace Demo.Inventory.Types;

[QueryType]
public static class Query
{
    public static string InventoryVersion2() => "2.0";

    [NodeResolver]
    [Lookup]
    public static async Task<InventoryItem?> GetInventoryItemByIdAsync(
        int id,
        InventoryItemByIdDataLoader loader,
        CancellationToken cancellationToken)
        => await loader.LoadAsync(id, cancellationToken);

    [Lookup]
    public static async Task<Product?> GetProductByIdAsync(
        [ID<Product>] int id,
        InventoryItemByProductIdDataLoader loader,
        CancellationToken cancellationToken)
    {
        var res = await loader.LoadAsync(id, cancellationToken);

        if (res is null)
        {
            return null!;
        }

        return new Product(res.Id, res.Quantity);
    }

    public static async Task<IReadOnlyList<Product>?> GetProductsByIdAsync(
        [ID<Product>] int[] ids,
        InventoryItemByProductIdDataLoader loader,
        CancellationToken cancellationToken)
    {
        var res = await loader.LoadAsync(ids, cancellationToken);

        if (res is null)
        {
            return null!;
        }

        return res.Select(x => new Product(x!.Id, x.Quantity)).ToList();
    }
}
