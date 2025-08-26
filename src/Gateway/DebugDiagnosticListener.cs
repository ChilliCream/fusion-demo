using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Channels;
using HotChocolate.Buffers;
using HotChocolate.Execution;
using HotChocolate.Execution.Instrumentation;
using HotChocolate.Fusion.Diagnostics;
using HotChocolate.Fusion.Execution;
using HotChocolate.Fusion.Execution.Nodes;
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

                        var eventId = errorEntry.Kind switch
                        {
                            ErrorKind.SyntaxError => EventIds.SyntaxError,
                            ErrorKind.ValidationError => EventIds.ValidationError,
                            ErrorKind.RequestError => EventIds.RequestError,
                            ErrorKind.FieldError => EventIds.FieldError,
                            ErrorKind.SubscriptionEventError => EventIds.SubscriptionEventError,
                            _ => EventIds.OtherError
                        };

                        _logger.LogError(eventId, message.ToString());
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

    public override IDisposable ExecuteOperation(
        RequestContext context)
        => new DiagnosticSpan(
            _backlog,
            $"Executed Operation {context.OperationDocumentInfo.Id.Value} in {{0}}",
            Stopwatch.GetTimestamp());

    public override IDisposable ExecuteOperationNode(OperationPlanContext context, OperationExecutionNode node)
        => new DiagnosticSpan(
            _backlog,
            $"Fetched from {node.SchemaName} operation {node.Operation.Name} in {{0}}",
            Stopwatch.GetTimestamp());


    public override IDisposable ExecuteIntrospectionNode(OperationPlanContext context, IntrospectionExecutionNode node)
        => new DiagnosticSpan(
            _backlog,
            $"Executed introspection in {{0}}",
            Stopwatch.GetTimestamp());

    public override void RetrievedDocumentFromCache(RequestContext context)
        => _backlog.Writer.TryWrite(new LogEntry($"Retrieved document from cache for {context.OperationDocumentInfo.Id.Value}"));

    public override void AddedOperationPlanToCache(RequestContext context, string operationPlanId)
        => _backlog.Writer.TryWrite(new LogEntry($"Added operation plan {operationPlanId} to cache for {context.OperationDocumentInfo.Id.Value}"));

    public override void ExecutionError(
        RequestContext context,
        ErrorKind kind,
        IReadOnlyList<IError> errors,
        object? state = null)
        => _backlog.Writer.TryWrite(new ErrorEntry(kind, errors));

    public void Dispose() => _backlog.Writer.TryComplete();

    private interface IEntry;

    public record LogEntry(string Message) : IEntry;

    private record ErrorEntry(
        ErrorKind Kind,
        IReadOnlyList<IError> Errors)
        : IEntry;

    private static class EventIds
    {
        public static readonly EventId SyntaxError = new(1001, "SyntaxError");
        public static readonly EventId ValidationError = new(1002, "ValidationError");
        public static readonly EventId RequestError = new(1003, "RequestError");
        public static readonly EventId FieldError = new(1004, "FieldError");
        public static readonly EventId SubscriptionEventError = new(1005, "SubscriptionEventError");
        public static readonly EventId OtherError = new(1006, "OtherError");
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
