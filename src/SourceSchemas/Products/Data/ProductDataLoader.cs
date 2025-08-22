namespace Demo.Products.Data;

internal static class ProductDataLoader
{
    [DataLoader]
    internal static async Task<Dictionary<int, Product>> GetProductByIdAsync(
        IReadOnlyList<int> ids,
        ProductContext context,
        CancellationToken cancellationToken)
        => await context.Products
            .Where(t => ids.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, cancellationToken);
}