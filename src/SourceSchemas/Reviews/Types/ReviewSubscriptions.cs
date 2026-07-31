namespace Demo.Reviews.Types;

[SubscriptionType]
public static partial class ReviewSubscriptions
{
    [EventStream("review { id }")]
    public static ReviewCreated OnReviewCreated([EventCursor] string? after)
        => EventStream.Create<ReviewCreated>(after);
}
