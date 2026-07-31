using Demo.Reviews.Types;
using NATS.Client.Core;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;

namespace Demo.Reviews;

/// <summary>
/// Ensures the JetStream stream that backs the review event streams exists. Reviews is the
/// producer that publishes to <see cref="ReviewTopics.ReviewCreated"/>, so it owns the durable
/// log. The Fusion gateway broker creates durable <em>consumers</em> on this stream on demand
/// but never creates the stream itself, so we provision it here at startup. The operation is
/// idempotent and safe to run on every start and across replicas.
/// </summary>
internal sealed class ReviewEventStreamInitializer(INatsConnection connection) : IHostedService
{
    // Must match the stream name configured on the gateway broker (NatsJetStreamOptions.Stream).
    private const string StreamName = "reviews";

    private static readonly string[] s_subjects = [ReviewTopics.ReviewCreated];

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var jetStream = new NatsJSContext(connection);

        await jetStream.CreateOrUpdateStreamAsync(
            new StreamConfig(StreamName, s_subjects),
            cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
