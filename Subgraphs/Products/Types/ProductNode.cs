namespace Demo.Products.Types;

[ExtendObjectType<Product>]
public static class ProductNode
{
    public static string Foo => "Hello";

    [DataLoader]
    internal static async Task<IReadOnlyDictionary<int, Product>> GetProductByIdAsync(
        IReadOnlyList<int> ids,
        ProductContext context,
        CancellationToken cancellationToken)
        => await context.Products
            .Where(t => ids.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, cancellationToken);

    public static ProductDimension GetDimension([Parent] Product product)
    {
        return product.Id switch
        {
            1 => new ProductDimension(250, 150),
            2 => new ProductDimension(2500, 150),
            3 => new ProductDimension(15, 30),
            _ => new ProductDimension(1, 1),
        };
    }
}
