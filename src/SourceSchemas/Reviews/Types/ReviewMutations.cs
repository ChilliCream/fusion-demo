using System.Text.Json;
using HotChocolate.Types.Relay;
using NATS.Client.Core;

namespace Demo.Reviews.Types;

[MutationType]
public static partial class ReviewMutations
{
    public static async Task<Review> CreateReview(
        CreateReviewInput input,
        ReviewContext context,
        INatsConnection nats,
        INodeIdSerializer nodeIdSerializer,
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

        var id = nodeIdSerializer.Format("Review", review.Id);
        var payload = JsonSerializer.SerializeToUtf8Bytes(new { review = new { id } });
        await nats.PublishAsync(
            ReviewTopics.ReviewCreated,
            payload,
            cancellationToken: cancellationToken);

        return review;
    }
}
