namespace Demo.Reviews.Types;

[QueryType]
public static partial class ReviewQueries
{
    [Lookup, NodeResolver]
    public static async Task<Review?> GetReviewByIdAsync(
        int id,
        IReviewByIdDataLoader reviewById,
        CancellationToken cancellationToken)
        => await reviewById.LoadAsync(id, cancellationToken);
}
