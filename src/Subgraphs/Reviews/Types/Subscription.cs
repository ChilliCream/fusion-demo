namespace Demo.Reviews.Types;

[SubscriptionType]
public static class Subscription
{
    [Subscribe]
    [Topic(nameof(Mutation.CreateReview))]
    public static async Task<Review?> OnCreateReview(
        [EventMessage] int reviewId,
        ReviewByIdDataLoader reviewById,
        CancellationToken cancellationToken)
        => await reviewById.LoadAsync(reviewId, cancellationToken);
}
