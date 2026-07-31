namespace Demo.Reviews.Types;

/// <summary>
/// Shared NATS subjects for the review event streams. The subgraph publishes to these
/// subjects and the Fusion gateway broker subscribes to them to fulfill federated
/// event-stream subscriptions.
/// </summary>
public static class ReviewTopics
{
    public const string ReviewCreated = "onReviewCreated";
}
