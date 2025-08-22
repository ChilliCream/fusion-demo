using HotChocolate.Subscriptions;

namespace Demo.Reviews.Types;

[MutationType]
public static partial class ReviewMutations
{
    public static async Task<Review> CreateReview(
        CreateReviewInput input,
        ReviewContext context,
        ITopicEventSender topicEventSender,
        CancellationToken cancellationToken)
    {
        var review = new Review
        {
            Body = input.Body,
            Stars = input.Stars,
            ProductId = input.ProductId,
            AuthorId = input.AuthorId
        };

        await context.Reviews.AddAsync(review, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await topicEventSender.SendAsync(nameof(CreateReview), review.Id, cancellationToken);

        return review;
    }
}