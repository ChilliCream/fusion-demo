using System.Collections.Immutable;
using System.IO.Pipelines;
using System.Text.Json;
using System.Threading.Channels;
using HotChocolate.Adapters.Mcp.Serialization;
using HotChocolate.Adapters.Mcp.Storage;
using HotChocolate.Buffers;
using HotChocolate.Language;
using HotChocolate.Utilities;

namespace Demo.Gateway.Mcp;

internal sealed class FileSystemMcpStorage : IMcpStorage, IAsyncDisposable
{
    private readonly Lock _syncRoot = new();
    private readonly FileSystemWatcher _watcher;
    private readonly string _promptsDirectoryPath;
    private readonly string _toolsDirectoryPath;

    private readonly Channel<IFileEvent> _fileUpdatedEvents = Channel.CreateBounded<IFileEvent>(
        new BoundedChannelOptions(10)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        });

    private readonly CancellationTokenSource _cts = new();
    private ImmutableArray<ObserverSession<PromptStorageEventArgs>> _promptSessions = [];
    private ImmutableArray<ObserverSession<OperationToolStorageEventArgs>> _toolSessions = [];
    private bool _disposed;

    public FileSystemMcpStorage(string directoryPath)
    {
        ArgumentException.ThrowIfNullOrEmpty(directoryPath);

        var fullPath = Path.GetFullPath(directoryPath);

        if (!Directory.Exists(fullPath))
        {
            throw new DirectoryNotFoundException($"Could not find directory '{fullPath}'");
        }

        _promptsDirectoryPath = Path.Combine(fullPath, "Prompts");
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

    public async ValueTask<IEnumerable<PromptDefinition>> GetPromptDefinitionsAsync(
        CancellationToken cancellationToken)
    {
        var jsonFiles = Directory.EnumerateFiles(
            _promptsDirectoryPath,
            "*.json",
            SearchOption.AllDirectories);

        var definitions = ImmutableArray.CreateBuilder<PromptDefinition>();

        foreach (var filePath in jsonFiles)
        {
            var directory = Path.GetDirectoryName(filePath)!;
            var parentDirectory = Path.GetDirectoryName(directory);

            if (parentDirectory != _promptsDirectoryPath)
            {
                continue;
            }

            var promptName = Path.GetFileNameWithoutExtension(filePath);
            definitions.Add(await CreatePromptDefinitionAsync(directory, promptName, cancellationToken));
        }

        return definitions.ToImmutable();
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
            var parentDirectory = Path.GetDirectoryName(directory);

            if (parentDirectory != _toolsDirectoryPath)
            {
                continue;
            }

            var toolName = Path.GetFileNameWithoutExtension(filePath);
            definitions.Add(await CreateOperationToolDefinitionAsync(directory, toolName, cancellationToken));
        }

        return definitions.ToImmutable();
    }

    public IDisposable Subscribe(IObserver<PromptStorageEventArgs> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);
        ObjectDisposedException.ThrowIf(_disposed, this);

        var session = new ObserverSession<PromptStorageEventArgs>(this, observer);

        lock (_syncRoot)
        {
            _promptSessions = _promptSessions.Add(session);
        }

        return session;
    }

    public IDisposable Subscribe(IObserver<OperationToolStorageEventArgs> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);
        ObjectDisposedException.ThrowIf(_disposed, this);

        var session = new ObserverSession<OperationToolStorageEventArgs>(this, observer);

        lock (_syncRoot)
        {
            _toolSessions = _toolSessions.Add(session);
        }

        return session;
    }

    private void Unsubscribe(ObserverSession<PromptStorageEventArgs> session)
    {
        lock (_syncRoot)
        {
            _promptSessions = _promptSessions.Remove(session);
        }
    }

    private void Unsubscribe(ObserverSession<OperationToolStorageEventArgs> session)
    {
        lock (_syncRoot)
        {
            _toolSessions = _toolSessions.Remove(session);
        }
    }

    private async Task FileUpdateProcessorAsync(CancellationToken cancellationToken)
    {
        await foreach (var @event in _fileUpdatedEvents.Reader.ReadAllAsync(cancellationToken))
        {
            if (@event.FilePath.StartsWith(_promptsDirectoryPath + Path.DirectorySeparatorChar))
            {
                await ProcessPromptFileEventAsync(@event, cancellationToken);
            }

            if (@event.FilePath.StartsWith(_toolsDirectoryPath + Path.DirectorySeparatorChar))
            {
                await ProcessToolFileEventAsync(@event, cancellationToken);
            }
        }
    }

    private async Task ProcessPromptFileEventAsync(IFileEvent @event, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(@event.FilePath);

        if (extension != ".json")
        {
            return;
        }

        var directory = Path.GetDirectoryName(@event.FilePath)!;
        var promptName = Path.GetFileName(directory);

        try
        {
            switch (@event)
            {
                case FileAddedEvent or FileModifiedEvent:
                    var eventType =
                        @event is FileAddedEvent
                            ? PromptStorageEventType.Added
                            : PromptStorageEventType.Modified;

                    var promptDefinition =
                        await CreatePromptDefinitionAsync(directory, promptName, cancellationToken);

                    NotifyObservers(
                        new PromptStorageEventArgs(
                            promptDefinition.Name,
                            eventType,
                            promptDefinition));

                    break;

                case FileRemovedEvent:
                    // The prompt has been removed.
                    NotifyObservers(
                        new PromptStorageEventArgs(promptName, PromptStorageEventType.Removed));

                    break;
            }
        }
        catch
        {
            // ignore and wait for next update
        }
    }

    private async Task ProcessToolFileEventAsync(IFileEvent @event, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(@event.FilePath);

        if (extension != ".graphql" && extension != ".html" && extension != ".json")
        {
            return;
        }

        var directory = Path.GetDirectoryName(@event.FilePath)!;
        var toolName = Path.GetFileName(directory);
        var graphqlFilePath = Path.Combine(directory, toolName + ".graphql");

        try
        {
            switch (@event)
            {
                case FileAddedEvent or FileModifiedEvent:
                {
                    // The GraphQL file should always exist.
                    if (!File.Exists(graphqlFilePath))
                    {
                        return;
                    }

                    var eventType =
                        @event is FileAddedEvent
                            ? OperationToolStorageEventType.Added
                            : OperationToolStorageEventType.Modified;

                    var operationToolDefinition =
                        await CreateOperationToolDefinitionAsync(directory, toolName, cancellationToken);

                    NotifyObservers(
                        new OperationToolStorageEventArgs(
                            operationToolDefinition.Name,
                            eventType,
                            operationToolDefinition));

                    break;
                }
                case FileRemovedEvent:
                {
                    if (File.Exists(graphqlFilePath))
                    {
                        // The component HTML or JSON file has been removed. Recreate the tool definition.
                        var operationToolDefinition =
                            await CreateOperationToolDefinitionAsync(directory, toolName, cancellationToken);

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

                    break;
                }
            }
        }
        catch
        {
            // ignore and wait for next update
        }
    }

    private static async Task<PromptDefinition> CreatePromptDefinitionAsync(
        string directory,
        string promptName,
        CancellationToken cancellationToken)
    {
        var settingsFilePath = Path.Combine(directory, promptName + ".json");

        var json = File.Exists(settingsFilePath)
            ? await File.ReadAllTextAsync(settingsFilePath, cancellationToken)
            : null;

        var jsonDocument = json is not null
            ? JsonDocument.Parse(json)
            : null;

        var promptSettings = jsonDocument is not null
            ? McpPromptSettingsSerializer.Parse(jsonDocument)
            : new McpPromptSettingsDto { Messages = [] };

        return PromptDefinition.From(promptName, promptSettings);
    }

    private static async Task<OperationToolDefinition> CreateOperationToolDefinitionAsync(
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

        var jsonDocument = json is not null
            ? JsonDocument.Parse(json)
            : null;

        var toolSettings = jsonDocument is not null
            ? McpToolSettingsSerializer.Parse(jsonDocument)
            : null;

        return OperationToolDefinition.From(document, toolName, toolSettings, html);
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

    private void NotifyObservers(PromptStorageEventArgs args)
    {
        ImmutableArray<ObserverSession<PromptStorageEventArgs>> sessions;

        lock (_syncRoot)
        {
            sessions = _promptSessions;
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

    private void NotifyObservers(OperationToolStorageEventArgs args)
    {
        ImmutableArray<ObserverSession<OperationToolStorageEventArgs>> sessions;

        lock (_syncRoot)
        {
            sessions = _toolSessions;
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
                foreach (var session in _toolSessions)
                {
                    session.Complete();
                }
            }

            while (_fileUpdatedEvents.Reader.TryRead(out _)) { }
        }
    }

    private sealed class ObserverSession<T>(
        FileSystemMcpStorage storage,
        IObserver<T> observer)
        : IDisposable
    {
        public void Notify(T args)
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

        public void Dispose()
        {
            switch (this)
            {
                case ObserverSession<OperationToolStorageEventArgs> toolSession:
                    storage.Unsubscribe(toolSession);
                    break;
                case ObserverSession<PromptStorageEventArgs> promptSession:
                    storage.Unsubscribe(promptSession);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported observer session type.");
            }
        }
    }
}

internal interface IFileEvent
{
    public string FilePath { get; }
}

internal sealed record FileAddedEvent(string FilePath) : IFileEvent;

internal sealed record FileModifiedEvent(string FilePath) : IFileEvent;

internal sealed record FileRemovedEvent(string FilePath) : IFileEvent;