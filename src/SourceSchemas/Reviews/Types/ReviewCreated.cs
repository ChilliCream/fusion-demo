using HotChocolate.Types.Composite;

namespace Demo.Reviews.Types;

/// <summary>
/// The event payload published when a review is created. The <see cref="Cursor"/> is
/// marked with <c>@eventCursor</c> so clients can resume the stream (replay) from a
/// known position when the broker is backed by a durable log such as NATS JetStream.
/// </summary>
public record ReviewCreated(
    Review Review,
    [property: EventCursor] string Cursor);
