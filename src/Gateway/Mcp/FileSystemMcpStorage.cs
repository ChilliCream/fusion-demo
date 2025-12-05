using System.Collections.Immutable;
using System.IO.Pipelines;
using System.Text.Json;
using System.Threading.Channels;
using HotChocolate.Adapters.Mcp.Storage;
using HotChocolate.Buffers;
using HotChocolate.Language;
using HotChocolate.Utilities;

namespace Demo.Gateway.Mcp;

internal sealed class FileSystemMcpStorage : IMcpStorage, IAsyncDisposable
{
    private readonly Lock _syncRoot = new();
    private readonly FileSystemWatcher _watcher;
    private readonly string _toolsDirectoryPath;
    private static readonly string[] Extensions = [".graphql", ".html", ".json"];

    private readonly Channel<IFileEvent> _fileUpdatedEvents = Channel.CreateBounded<IFileEvent>(
        new BoundedChannelOptions(10)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        });

    private readonly CancellationTokenSource _cts = new();
    private ImmutableArray<ObserverSession> _sessions = [];
    private bool _disposed;

    public FileSystemMcpStorage(string directoryPath)
    {
        ArgumentException.ThrowIfNullOrEmpty(directoryPath);

        var fullPath = Path.GetFullPath(directoryPath);

        if (!Directory.Exists(fullPath))
        {
            throw new DirectoryNotFoundException($"Could not find directory '{fullPath}'");
        }

        _toolsDirectoryPath = Path.Combine(fullPath, "Tools");

        _watcher = new FileSystemWatcher
        {
            Path = fullPath,
            Filter = "*.*",
            NotifyFilter =
                NotifyFilters.FileName |
                NotifyFilters.DirectoryName |
                NotifyFilters.Attributes |
                NotifyFilters.CreationTime |
                NotifyFilters.LastWrite |
                NotifyFilters.Size,
            IncludeSubdirectories = true
        };

        _watcher.Created += (_, e) => _fileUpdatedEvents.Writer.TryWrite(new FileAddedEvent(e.FullPath));
        _watcher.Changed += (_, e) => _fileUpdatedEvents.Writer.TryWrite(new FileModifiedEvent(e.FullPath));
        _watcher.Deleted += (_, e) => _fileUpdatedEvents.Writer.TryWrite(new FileRemovedEvent(e.FullPath));

        _watcher.EnableRaisingEvents = true;

        FileUpdateProcessorAsync(_cts.Token).FireAndForget();
    }

    public async ValueTask<IEnumerable<OperationToolDefinition>> GetOperationToolDefinitionsAsync(
        CancellationToken cancellationToken)
    {
        var graphqlFiles = Directory.EnumerateFiles(
            _toolsDirectoryPath,
            "*.graphql",
            SearchOption.AllDirectories);

        var definitions = ImmutableArray.CreateBuilder<OperationToolDefinition>();

        foreach (var filePath in graphqlFiles)
        {
            var directory = Path.GetDirectoryName(filePath)!;
            var fileBaseName = Path.GetFileNameWithoutExtension(filePath);
            definitions.Add(await CreateOperationToolDefinition(directory, fileBaseName, cancellationToken));
        }

        return definitions.ToImmutable();
    }

    public IDisposable Subscribe(IObserver<OperationToolStorageEventArgs> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);
        ObjectDisposedException.ThrowIf(_disposed, this);

        var session = new ObserverSession(this, observer);

        lock (_syncRoot)
        {
            _sessions = _sessions.Add(session);
        }

        return session;
    }

    private void Unsubscribe(ObserverSession session)
    {
        lock (_syncRoot)
        {
            _sessions = _sessions.Remove(session);
        }
    }

    private async Task FileUpdateProcessorAsync(CancellationToken cancellationToken)
    {
        await foreach (var @event in _fileUpdatedEvents.Reader.ReadAllAsync(cancellationToken))
        {
            // Filter by supported extensions (graphql, html, json).
            if (!Extensions.Contains(Path.GetExtension(@event.FilePath)))
            {
                continue;
            }

            var directory = Path.GetDirectoryName(@event.FilePath)!;

            // Only process changes in the Tools directory.
            if (!directory.StartsWith(_toolsDirectoryPath + Path.DirectorySeparatorChar))
            {
                continue;
            }

            var toolName = Path.GetFileName(directory);
            var graphqlFilePath = Path.Combine(directory, toolName + ".graphql");

            try
            {
                if (@event is FileAddedEvent or FileModifiedEvent)
                {
                    // The GraphQL file should always exist.
                    if (!File.Exists(graphqlFilePath))
                    {
                        continue;
                    }

                    var eventType =
                        @event is FileAddedEvent
                            ? OperationToolStorageEventType.Added
                            : OperationToolStorageEventType.Modified;

                    var operationToolDefinition =
                        await CreateOperationToolDefinition(directory, toolName, cancellationToken);

                    NotifyObservers(
                        new OperationToolStorageEventArgs(
                            operationToolDefinition.Name,
                            eventType,
                            operationToolDefinition));
                }
                else if (@event is FileRemovedEvent)
                {
                    if (File.Exists(graphqlFilePath))
                    {
                        // The component HTML or JSON file has been removed. Recreate the tool definition.
                        var operationToolDefinition =
                            await CreateOperationToolDefinition(directory, toolName, cancellationToken);

                        NotifyObservers(
                            new OperationToolStorageEventArgs(
                                operationToolDefinition.Name,
                                OperationToolStorageEventType.Modified,
                                operationToolDefinition));
                    }
                    else
                    {
                        // The tool has been removed.
                        NotifyObservers(
                            new OperationToolStorageEventArgs(
                                toolName,
                                OperationToolStorageEventType.Removed));
                    }
                }
            }
            catch
            {
                // ignore and wait for next update
            }
        }
    }

    private static async Task<OperationToolDefinition> CreateOperationToolDefinition(
        string directory,
        string toolName,
        CancellationToken cancellationToken)
    {
        var graphqlFilePath = Path.Combine(directory, toolName + ".graphql");

        await using var fileStream = File.OpenRead(graphqlFilePath);
        var document = await ReadDocumentAsync(fileStream, cancellationToken);

        // Read optional HTML and JSON files.
        var htmlFilePath = Path.Combine(directory, toolName + ".html");
        var jsonFilePath = Path.Combine(directory, toolName + ".json");

        var html = File.Exists(htmlFilePath)
            ? await File.ReadAllTextAsync(htmlFilePath, cancellationToken)
            : null;

        var json = File.Exists(jsonFilePath)
            ? await File.ReadAllTextAsync(jsonFilePath, cancellationToken)
            : null;

        var jsonSettings = json is not null
            ? JsonSerializer.Deserialize(json, McpToolSettingsJsonSerializerContext.Default.McpToolSettings)
            : null;

        return new OperationToolDefinition(document)
        {
            Name = toolName,
            Title = jsonSettings?.Title,
            Icons =
                jsonSettings?.Icons?.Select(
                    i => new OperationToolIcon(i.Source)
                    {
                        MimeType = i.MimeType,
                        Sizes = i.Sizes,
                        Theme = i.Theme
                    }).ToImmutableArray(),
            DestructiveHint = jsonSettings?.Annotations?.DestructiveHint,
            IdempotentHint = jsonSettings?.Annotations?.IdempotentHint,
            OpenWorldHint = jsonSettings?.Annotations?.OpenWorldHint,
            OpenAiComponent = html is null ? null : new OpenAiComponent(html)
            {
                Csp = jsonSettings?.OpenAiComponent?.Csp is { } csp
                    ? new OpenAiComponentCsp(csp.ConnectDomains, csp.ResourceDomains)
                    : null,
                Description = jsonSettings?.OpenAiComponent?.Description,
                Domain = jsonSettings?.OpenAiComponent?.Domain,
                PrefersBorder = jsonSettings?.OpenAiComponent?.PrefersBorder,
                ToolInvokingStatusText = jsonSettings?.OpenAiComponent?.ToolInvokingStatusText,
                ToolInvokedStatusText = jsonSettings?.OpenAiComponent?.ToolInvokedStatusText,
                AllowToolCalls = jsonSettings?.OpenAiComponent?.AllowToolCalls ?? false
            }
        };
    }

    private static async ValueTask<DocumentNode> ReadDocumentAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        using var buffer = new PooledArrayWriter();
        var pipeReader = PipeReader.Create(stream);

        while (true)
        {
            var result = await pipeReader.ReadAsync(cancellationToken);
            var readBuffer = result.Buffer;

            foreach (var segment in readBuffer)
            {
                var span = segment.Span;
                span.CopyTo(buffer.GetSpan(span.Length));
                buffer.Advance(span.Length);
            }

            pipeReader.AdvanceTo(readBuffer.End);

            if (result.IsCompleted)
            {
                break;
            }
        }

        await pipeReader.CompleteAsync();

        var document = Utf8GraphQLParser.Parse(buffer.WrittenSpan);
        return document;
    }

    private void NotifyObservers(OperationToolStorageEventArgs args)
    {
        ImmutableArray<ObserverSession> sessions;

        lock (_syncRoot)
        {
            sessions = _sessions;
        }

        if (sessions.IsEmpty)
        {
            return;
        }

        foreach (var session in sessions)
        {
            session.Notify(args);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            _disposed = true;

            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();

            await _cts.CancelAsync();
            _cts.Dispose();

            lock (_syncRoot)
            {
                foreach (var session in _sessions)
                {
                    session.Complete();
                }
            }

            while (_fileUpdatedEvents.Reader.TryRead(out _)) { }
        }
    }

    private sealed class ObserverSession(
        FileSystemMcpStorage storage,
        IObserver<OperationToolStorageEventArgs> observer)
        : IDisposable
    {
        public void Notify(OperationToolStorageEventArgs args)
        {
            try
            {
                observer.OnNext(args);
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
            }
        }

        public void Complete()
        {
            try
            {
                observer.OnCompleted();
            }
            catch
            {
                // We do not want to throw an exception if the observer
                // throws an exception on completion.
            }
        }

        public void Dispose() => storage.Unsubscribe(this);
    }
}

internal interface IFileEvent
{
    public string FilePath { get; }
}

internal sealed record FileAddedEvent(string FilePath) : IFileEvent;

internal sealed record FileModifiedEvent(string FilePath) : IFileEvent;

internal sealed record FileRemovedEvent(string FilePath) : IFileEvent;