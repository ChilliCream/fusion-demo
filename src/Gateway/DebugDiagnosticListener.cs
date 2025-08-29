using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Channels;
using HotChocolate.Buffers;
using HotChocolate.Execution;
using HotChocolate.Fusion.Diagnostics;
using HotChocolate.Fusion.Execution;
using HotChocolate.Fusion.Execution.Nodes;
using HotChocolate.Language;
using HotChocolate.Utilities;

namespace HotChocolate.Fusion.AspNetCore;

internal sealed class DebugDiagnosticListener
    : FusionExecutionDiagnosticEventListener
    , IDisposable
{
    private readonly ILogger _logger;
    private readonly Channel<IEntry> _backlog =
        Channel.CreateBounded<IEntry>(
            new BoundedChannelOptions(16)
            {
                SingleReader = true,
                FullMode = BoundedChannelFullMode.Wait
            });

    public DebugDiagnosticListener(ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        _logger = loggerFactory.CreateLogger("HotChocolate.Fusion");
        WriteToLogAsync().FireAndForget();
    }

    private async Task WriteToLogAsync()
    {
        var serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var writerOptions = new JsonWriterOptions
        {
            Indented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        using var buffer = new PooledArrayWriter(4096);
        await using var jsonWriter = new Utf8JsonWriter(buffer, writerOptions);
        var message = new StringBuilder();

        await foreach (var entry in _backlog.Reader.ReadAllAsync())
        {
            switch (entry)
            {
                case LogEntry logEntry:
                    _logger.LogInformation(logEntry.Message);
                    Console.WriteLine(logEntry.Message);
                    break;

                case ErrorEntry errorEntry:
                    foreach (var error in errorEntry.Errors)
                    {
                        message.Clear();
                        buffer.Reset();
                        jsonWriter.Reset();

                        JsonValueFormatter.WriteError(
                            jsonWriter,
                            error,
                            serializerOptions,
                            JsonNullIgnoreCondition.None);

                        message.AppendLine(Encoding.UTF8.GetString(buffer.WrittenSpan));

                        if (error.Exception is not null)
                        {
                            message.AppendLine(error.Exception.Message);
                            message.AppendLine(error.Exception.GetType().FullName);
                            message.Append(error.Exception.StackTrace);
                        }

                        _logger.LogError(errorEntry.Id, message.ToString());
                        Console.WriteLine(message.ToString());
                    }
                    break;
            }
        }
    }

    public override IDisposable PlanOperation(
        RequestContext context,
        string operationPlanId)
        => new DiagnosticSpan(
            _backlog,
            $"Planned Operation {operationPlanId} for {{0}}",
            Stopwatch.GetTimestamp());

    public override void RetrievedOperationPlanFromCache(RequestContext context, string operationPlanId)
        => _backlog.Writer.TryWrite(new LogEntry($"Retrieved operation plan {operationPlanId} from cache for {context.OperationDocumentInfo.Id.Value}"));

    public override void AddedOperationPlanToCache(RequestContext context, string operationPlanId)
        => _backlog.Writer.TryWrite(new LogEntry($"Added operation plan {operationPlanId} to cache for {context.OperationDocumentInfo.Id.Value}"));

    public override IDisposable ExecuteSubscription(RequestContext context, ulong subscriptionId)
        => new DiagnosticSpan(
            _backlog,
            $"Executed Subscription {subscriptionId} for {{0}}",
            Stopwatch.GetTimestamp());

    public override IDisposable ExecuteSubscriptionNode(OperationPlanContext context, OperationExecutionNode node, string schemaName, ulong subscriptionId)
        => new DiagnosticSpan(
            _backlog,
            $"Fetched from {schemaName} subscription {subscriptionId} operation {node.Operation.Name} in {{0}}",
            Stopwatch.GetTimestamp());

    public override IDisposable ExecuteOperation(
        RequestContext context)
        => new DiagnosticSpan(
            _backlog,
            $"Executed Operation {context.OperationDocumentInfo.Id.Value} in {{0}}",
            Stopwatch.GetTimestamp());

    public override IDisposable ExecuteOperationNode(OperationPlanContext context, OperationExecutionNode node, string schemaName)
        => new DiagnosticSpan(
            _backlog,
            $"Fetched from {schemaName} operation {node.Operation.Name} in {{0}}",
            Stopwatch.GetTimestamp());


    public override IDisposable ExecuteIntrospectionNode(OperationPlanContext context, IntrospectionExecutionNode node)
        => new DiagnosticSpan(
            _backlog,
            $"Executed introspection in {{0}}",
            Stopwatch.GetTimestamp());

    public override void RetrievedDocumentFromCache(RequestContext context)
        => _backlog.Writer.TryWrite(new LogEntry($"Retrieved document from cache for {context.OperationDocumentInfo.Id.Value}"));

    public override void AddedDocumentToCache(RequestContext context)
        => _backlog.Writer.TryWrite(new LogEntry($"Added document to cache for {context.OperationDocumentInfo.Id.Value}"));

    public override void RequestError(RequestContext context, Exception error)
    {
        var errorObj = ErrorBuilder.FromException(error).SetMessage(error.Message).Build();
        _backlog.Writer.TryWrite(new ErrorEntry(EventIds.RequestError, [errorObj]));
    }

    public override void RequestError(RequestContext context, IError error)
        => _backlog.Writer.TryWrite(new ErrorEntry(EventIds.RequestError, [error]));

    public override void ValidationErrors(RequestContext context, IReadOnlyList<IError> errors)
        => _backlog.Writer.TryWrite(new ErrorEntry(EventIds.ValidationError, errors));

    public override void DocumentNotFoundInStorage(RequestContext context, OperationDocumentId documentId)
    {
        var error = new Error { Message = $"The document with id '{documentId}' was not found in the document storage." };
        _backlog.Writer.TryWrite(new ErrorEntry(EventIds.PersistedOperations, [error]));
    }

    public override void UntrustedDocumentRejected(RequestContext context)
    {
        var error = new Error { Message = "The provided document is not trusted and was rejected." };
        _backlog.Writer.TryWrite(new ErrorEntry(EventIds.PersistedOperations, [error]));
    }

    public override void SourceSchemaTransportError(
        OperationPlanContext context,
        ExecutionNode node,
        string schemaName,
        Exception error)
    {
        var errorObj = ErrorBuilder
            .FromException(error)
            .SetMessage($"A transport error occurred while fetching data from the source schema '{schemaName}'.")
            .Build();
        _backlog.Writer.TryWrite(new ErrorEntry(EventIds.SourceSchemaTransportError, [errorObj]));
    }

    public override void SourceSchemaStoreError(
        OperationPlanContext context,
        ExecutionNode node,
        string schemaName,
        Exception error)
    {
        var errorObj = ErrorBuilder
            .FromException(error)
            .SetMessage($"An error occurred while writing the source schema result from '{schemaName}' to the result store.")
            .Build();
        _backlog.Writer.TryWrite(new ErrorEntry(EventIds.SourceSchemaStoreError, [errorObj]));
    }

    public override void SourceSchemaResultError(
        OperationPlanContext context,
        ExecutionNode node,
        string schemaName,
        IReadOnlyCollection<IError> errors)
        => _backlog.Writer.TryWrite(new ErrorEntry(EventIds.SourceSchemaResultError, [.. errors]));

    public override void SubscriptionTransportError(
        OperationPlanContext context,
        ExecutionNode node,
        string schemaName,
        ulong subscriptionId,
        Exception exception)
    {
        var errorObj = ErrorBuilder
            .FromException(exception)
            .SetMessage($"A transport error occurred while processing the subscription '{subscriptionId}' for schema '{schemaName}'.")
            .Build();
        _backlog.Writer.TryWrite(new ErrorEntry(EventIds.SubscriptionTransportError, [errorObj]));
    }

    public override void SubscriptionEventError(OperationPlanContext context, ExecutionNode node, string schemaName, ulong subscriptionId, Exception exception)
    {
        var errorObj = ErrorBuilder
            .FromException(exception)
            .SetMessage($"An error occurred while processing the subscription event '{subscriptionId}' for schema '{schemaName}'.")
            .Build();
        _backlog.Writer.TryWrite(new ErrorEntry(EventIds.SubscriptionEventError, [errorObj]));
    }

    public void Dispose() => _backlog.Writer.TryComplete();

    private interface IEntry;

    public record LogEntry(string Message) : IEntry;

    private record ErrorEntry(
        EventId Id,
        IReadOnlyList<IError> Errors)
        : IEntry;

    private static class EventIds
    {
        public static readonly EventId ValidationError = new(1002, "ValidationError");
        public static readonly EventId RequestError = new(1003, "RequestError");
        public static readonly EventId FieldError = new(1004, "FieldError");
        public static readonly EventId SourceSchemaTransportError = new(1005, "SourceSchemaTransportError");
        public static readonly EventId SourceSchemaStoreError = new(1006, "SourceSchemaStoreError");
        public static readonly EventId SourceSchemaResultError = new(1007, "SourceSchemaResultError");
        public static readonly EventId ExecutionNodeError = new(1008, "ExecutionNodeError");
        public static readonly EventId SubscriptionTransportError = new(1009, "SubscriptionTransportError");
        public static readonly EventId SubscriptionEventError = new(1010, "SubscriptionEventError");
        public static readonly EventId PersistedOperations = new(1011, "PersistedOperations");
    }

    private sealed class DiagnosticSpan(
        ChannelWriter<IEntry> writer,
        string messageTemplate,
        long start)
        : IDisposable
    {
        public void Dispose()
        {
            var message = string.Format(messageTemplate, Stopwatch.GetElapsedTime(start));
            writer.TryWrite(new LogEntry(message));
        }
    }
}
