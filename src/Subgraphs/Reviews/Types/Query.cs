namespace Demo.Reviews.Types;

[QueryType]
public static partial class Query
{
    [Lookup, NodeResolver]
    public static async Task<Review?> GetReviewByIdAsync(
        int id,
        ReviewByIdDataLoader reviewById,
        CancellationToken cancellationToken)
        => await reviewById.LoadAsync(id, cancellationToken);

    [Lookup, Internal]
    public static async Task<User?> GetUserById(
        [ID<Product>] int id,
        UserByIdDataLoader userById,
        CancellationToken cancellationToken)
        => await userById.LoadAsync(id, cancellationToken);

    [Lookup, Internal]
    public static Product GetProductById([ID<Product>] int id)
        => new(id);
}
