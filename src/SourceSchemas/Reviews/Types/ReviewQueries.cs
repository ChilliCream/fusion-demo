namespace Demo.Reviews.Types;

[QueryType]
public static partial class ReviewQueries
{
    [Lookup, NodeResolver]
    public static async Task<Review?> GetReviewByIdAsync(
        int id,
        ReviewByIdDataLoader reviewById,
        CancellationToken cancellationToken)
        => await reviewById.LoadAsync(id, cancellationToken);
}
