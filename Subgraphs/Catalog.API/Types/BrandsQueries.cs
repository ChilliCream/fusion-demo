using System.Runtime.CompilerServices;
using eShop.Catalog.Data.Entities;

[QueryType]
public static class BrandsQueries
{
    [Lookup]
    public static async Task<Brand?> GetBrandByIdAsync(
        int id,
        CatalogContext context,
        CancellationToken cancellationToken)
        => await context.Brands.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
}


[SubscriptionType]
public static class BrandSubscriptions
{
    public static async IAsyncEnumerable<Brand> OnBrandAddedStreamAsync(
        CatalogContext context,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var brand in context.Brands.Include(t => t.Products))
        {
            yield return brand;

            await Task.Delay(1000, cancellationToken);
        }
    }

    [Subscribe(With = nameof(OnBrandAddedStreamAsync))]
    public static Brand OnBrandAdded([EventMessage] Brand brand)
        => brand;
}