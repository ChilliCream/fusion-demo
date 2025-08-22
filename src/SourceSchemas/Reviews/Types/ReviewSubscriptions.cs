namespace Demo.Reviews.Types;

[SubscriptionType]
public static partial class ReviewSubscriptions
{
    [Subscribe]
    [Topic(nameof(ReviewMutations.CreateReview))]
    public static async Task<Review?> OnCreateReview(
        [EventMessage] int reviewId,
        ReviewByIdDataLoader reviewById,
        CancellationToken cancellationToken)
        => await reviewById.LoadAsync(reviewId, cancellationToken);
}
