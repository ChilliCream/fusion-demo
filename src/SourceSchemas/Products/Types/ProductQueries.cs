using GreenDonut.Data;
using HotChocolate.Types.Pagination;

namespace Demo.Products.Types;

[QueryType]
public static partial class ProductQueries
{
    [Tag("team-products")]
    [NodeResolver]
    public static async Task<Product?> GetProductByIdAsync(
        int id,
        IProductByIdDataLoader productById,
        CancellationToken cancellationToken)
        => await productById.LoadAsync(id, cancellationToken);

    [Tag("team-products")]
    [UsePaging]
    public static async Task<Connection<Product>> GetProducts(
        string? searchText,
        double? minPrice,
        double? maxPrice,
        PagingArguments arguments,
        ProductContext context,
        CancellationToken cancellationToken)
    {
        IQueryable<Product> query = context.Products;

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(t => EF.Functions.ILike(t.Name!, $"%{searchText}%"));
        }

        if (minPrice.HasValue)
        {
            var p = minPrice.Value;
            query = query.Where(t => t.Price >= p);
        }

        if (maxPrice.HasValue)
        {
            var p = maxPrice.Value;
            query = query.Where(t => t.Price <= p);
        }

        return await query
            .OrderBy(t => t.Name)
            .ThenBy(t => t.Id)
            .ToPageAsync(arguments, cancellationToken)
            .ToConnectionAsync();
    }
}
