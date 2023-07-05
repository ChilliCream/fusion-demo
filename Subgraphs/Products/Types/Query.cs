namespace Demo.Products.Types;

[QueryType]
public static class Query
{
    [NodeResolver]
    public static async Task<Product?> GetProductByIdAsync(
        int id,
        ProductByIdDataLoader productById,
        CancellationToken cancellationToken)
        => await productById.LoadAsync(id, cancellationToken);

    [UsePaging]
    public static IQueryable<Product> GetProducts(
        ProductContext context)
        => context.Products.OrderBy(t => t.Name);
}