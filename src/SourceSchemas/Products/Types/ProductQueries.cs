using GreenDonut.Data;
using HotChocolate.Types.Pagination;

namespace Demo.Products.Types;

[QueryType]
public static partial class ProductQueries
{
    [NodeResolver, Lookup]
    public static async Task<Product?> GetProductByIdAsync(
        int id,
        ProductByIdDataLoader productById,
        CancellationToken cancellationToken)
        => await productById.LoadAsync(id, cancellationToken);

    [UsePaging]
    public static async Task<Connection<Product>> GetProducts(
        PagingArguments arguments,
        ProductContext context,
        CancellationToken cancellationToken)
        => await context.Products
            .OrderBy(t => t.Name)
            .ThenBy(t => t.Id)
            .ToPageAsync(arguments, cancellationToken)
            .ToConnectionAsync();
}
