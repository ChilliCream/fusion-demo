using eShop.Catalog.Data.Entities;
using HotChocolate.Types.Pagination;
using GreenDonut.Data;

[QueryType]
public static class ProductsQueries
{
    [Lookup]
    public static async Task<Product?> GetProductByIdAsync(
        int id,
        QueryContext<Product> query,
        CatalogContext context,
        CancellationToken cancellationToken)
        => await context.Products
            .With(query.Include(t => t.Id))
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    [UsePaging]
    [UseFiltering]
    public static async Task<Connection<Product>> GetProductsAsync(
        PagingArguments pagingArgs,
        QueryContext<Product> query,
        CatalogContext context,
        CancellationToken cancellationToken)
        => await context.Products
            .OrderBy(t => t.Name)
            .ThenBy(t => t.Id)
            .With(query)
            .ToPageAsync(pagingArgs, cancellationToken)
            .ToConnectionAsync();
}