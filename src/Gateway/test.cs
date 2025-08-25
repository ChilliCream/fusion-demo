using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Channels;
using HotChocolate.Buffers;
using HotChocolate.Execution;
using HotChocolate.Execution.Instrumentation;
using HotChocolate.Fusion.Diagnostics;
using HotChocolate.Utilities;

namespace HotChocolate.Fusion.AspNetCore;

internal sealed class DevLog
    : FusionExecutionDiagnosticEventListener
    , IDisposable
{
    private readonly ILogger _logger;
    private readonly Channel<ErrorEntry> _backlog =
        Channel.CreateBounded<ErrorEntry>(
            new BoundedChannelOptions(16)
            {
                SingleReader = true,
                FullMode = BoundedChannelFullMode.Wait
            });

    public DevLog(ILoggerFactory loggerFactory)
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
            foreach (var error in entry.Errors)
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

                var eventId = entry.Kind switch
                {
                    ErrorKind.SyntaxError => EventIds.SyntaxError,
                    ErrorKind.ValidationError => EventIds.ValidationError,
                    ErrorKind.RequestError => EventIds.RequestError,
                    ErrorKind.FieldError => EventIds.FieldError,
                    ErrorKind.SubscriptionEventError => EventIds.SubscriptionEventError,
                    _ => EventIds.OtherError
                };

                _logger.LogError(eventId, message.ToString());
            }
        }
    }

    public override IDisposable ExecuteOperation(RequestContext context)
    {

        Console.WriteLine("ExecuteOperation");
        return base.ExecuteOperation(context);
    }

    public override void ExecutionError(
        RequestContext context,
        ErrorKind kind,
        IReadOnlyList<IError> errors,
        object? state = null)
        => _backlog.Writer.TryWrite(new ErrorEntry(kind, errors));

    

    public void Dispose() => _backlog.Writer.TryComplete();

    private record ErrorEntry(
        ErrorKind Kind,
        IReadOnlyList<IError> Errors);

    private static class EventIds
    {
        public static readonly EventId SyntaxError = new(1001, "SyntaxError");
        public static readonly EventId ValidationError = new(1002, "ValidationError");
        public static readonly EventId RequestError = new(1003, "RequestError");
        public static readonly EventId FieldError = new(1004, "FieldError");
        public static readonly EventId SubscriptionEventError = new(1005, "SubscriptionEventError");
        public static readonly EventId OtherError = new(1006, "OtherError");
    }
}
