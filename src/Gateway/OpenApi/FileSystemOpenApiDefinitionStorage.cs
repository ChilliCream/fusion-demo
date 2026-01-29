// using System.Collections.Immutable;
// using System.IO.Pipelines;
// using System.Threading.Channels;
// using HotChocolate.Adapters.OpenApi;
// using HotChocolate.Buffers;
// using HotChocolate.Language;
// using HotChocolate.Utilities;
//
// internal interface IFileEvent
// {
//     public string FilePath { get; }
// }
//
// internal sealed record FileAddedEvent(string FilePath) : IFileEvent;
//
// internal sealed record FileModifiedEvent(string FilePath) : IFileEvent;
//
// internal sealed record FileRemovedEvent(string FilePath) : IFileEvent;
//
// internal sealed class FileSystemOpenApiDefinitionStorage
//     : IOpenApiDefinitionStorage
//     , IAsyncDisposable
// {
// #if NET9_0_OR_GREATER
//     private readonly Lock _syncRoot = new();
// #else
//     private readonly object _syncRoot = new();
// #endif
//     private readonly FileSystemWatcher _watcher;
//     private readonly string _directoryPath;
//
//     private readonly Channel<IFileEvent> _fileUpdatedEvents = Channel.CreateBounded<IFileEvent>(
//         new BoundedChannelOptions(10)
//         {
//             FullMode = BoundedChannelFullMode.Wait,
//             SingleReader = true,
//             SingleWriter = false,
//         }
//     );
//
//     private readonly CancellationTokenSource _cts = new();
//     private ImmutableArray<ObserverSession> _sessions = [];
//     private bool _disposed;
//
//     public FileSystemOpenApiDefinitionStorage(string fileName)
//     {
//         ArgumentException.ThrowIfNullOrEmpty(fileName);
//
//         var fullPath = Path.GetFullPath(fileName);
//
//         if (!Directory.Exists(fullPath))
//         {
//             throw new DirectoryNotFoundException($"Could not find directory '{fullPath}'");
//         }
//
//         _directoryPath = fullPath;
//
//         _watcher = new FileSystemWatcher
//         {
//             Path = _directoryPath,
//             Filter = "*.graphql",
//             NotifyFilter =
//                 NotifyFilters.FileName
//                 | NotifyFilters.DirectoryName
//                 | NotifyFilters.Attributes
//                 | NotifyFilters.CreationTime
//                 | NotifyFilters.LastWrite
//                 | NotifyFilters.Size,
//         };
//
//         _watcher.Created += (_, e) =>
//         {
//             _fileUpdatedEvents.Writer.TryWrite(new FileAddedEvent(e.FullPath));
//         };
//
//         _watcher.Changed += (_, e) =>
//         {
//             _fileUpdatedEvents.Writer.TryWrite(new FileModifiedEvent(e.FullPath));
//         };
//
//         _watcher.Deleted += (_, e) =>
//         {
//             _fileUpdatedEvents.Writer.TryWrite(new FileRemovedEvent(e.FullPath));
//         };
//
//         _watcher.EnableRaisingEvents = true;
//
//         FileUpdateProcessorAsync(_cts.Token).FireAndForget();
//     }
//
//     public async ValueTask<IEnumerable<OpenApiDocumentDefinition>> GetDocumentsAsync(
//         CancellationToken cancellationToken
//     )
//     {
//         var graphqlFiles = Directory.EnumerateFiles(
//             _directoryPath,
//             "*.graphql",
//             SearchOption.AllDirectories
//         );
//
//         var definitions = ImmutableArray.CreateBuilder<OpenApiDocumentDefinition>();
//
//         foreach (var filePath in graphqlFiles)
//         {
//             await using var fileStream = File.OpenRead(filePath);
//             var document = await ReadDocumentAsync(fileStream, cancellationToken);
//             definitions.Add(new OpenApiDocumentDefinition(filePath, document));
//         }
//
//         return definitions.ToImmutable();
//     }
//
//     public IDisposable Subscribe(IObserver<OpenApiDefinitionStorageEventArgs> observer)
//     {
//         ArgumentNullException.ThrowIfNull(observer);
//         ObjectDisposedException.ThrowIf(_disposed, this);
//
//         var session = new ObserverSession(this, observer);
//
//         lock (_syncRoot)
//         {
//             _sessions = _sessions.Add(session);
//         }
//
//         return session;
//     }
//
//     private void Unsubscribe(ObserverSession session)
//     {
//         lock (_syncRoot)
//         {
//             _sessions = _sessions.Remove(session);
//         }
//     }
//
//     private async Task FileUpdateProcessorAsync(CancellationToken cancellationToken)
//     {
//         await foreach (var @event in _fileUpdatedEvents.Reader.ReadAllAsync(cancellationToken))
//         {
//             try
//             {
//                 if (@event is FileAddedEvent or FileModifiedEvent)
//                 {
//                     await using var fileStream = File.OpenRead(@event.FilePath);
//                     var document = await ReadDocumentAsync(fileStream, cancellationToken);
//
//                     var eventType =
//                         @event is FileAddedEvent
//                             ? OpenApiDefinitionStorageEventType.Added
//                             : OpenApiDefinitionStorageEventType.Modified;
//
//                     NotifyObservers(
//                         new OpenApiDefinitionStorageEventArgs(
//                             @event.FilePath,
//                             eventType,
//                             new OpenApiDocumentDefinition(@event.FilePath, document)
//                         )
//                     );
//                 }
//                 else if (@event is FileRemovedEvent)
//                 {
//                     NotifyObservers(
//                         new OpenApiDefinitionStorageEventArgs(
//                             @event.FilePath,
//                             OpenApiDefinitionStorageEventType.Removed
//                         )
//                     );
//                 }
//             }
//             catch
//             {
//                 // ignore and wait for next update
//             }
//         }
//     }
//
//     private async ValueTask<DocumentNode> ReadDocumentAsync(
//         Stream stream,
//         CancellationToken cancellationToken
//     )
//     {
//         using var buffer = new PooledArrayWriter();
//         var pipeReader = PipeReader.Create(stream);
//
//         while (true)
//         {
//             var result = await pipeReader.ReadAsync(cancellationToken);
//             var readBuffer = result.Buffer;
//
//             foreach (var segment in readBuffer)
//             {
//                 var span = segment.Span;
//                 span.CopyTo(buffer.GetSpan(span.Length));
//                 buffer.Advance(span.Length);
//             }
//
//             pipeReader.AdvanceTo(readBuffer.End);
//
//             if (result.IsCompleted)
//             {
//                 break;
//             }
//         }
//
//         await pipeReader.CompleteAsync();
//
//         var document = Utf8GraphQLParser.Parse(buffer.WrittenSpan);
//         return document;
//     }
//
//     private void NotifyObservers(OpenApiDefinitionStorageEventArgs args)
//     {
//         ImmutableArray<ObserverSession> sessions;
//
//         lock (_syncRoot)
//         {
//             sessions = _sessions;
//         }
//
//         if (sessions.IsEmpty)
//         {
//             return;
//         }
//
//         foreach (var session in sessions)
//         {
//             session.Notify(args);
//         }
//     }
//
//     public async ValueTask DisposeAsync()
//     {
//         if (!_disposed)
//         {
//             _disposed = true;
//
//             _watcher.EnableRaisingEvents = false;
//             _watcher.Dispose();
//
//             await _cts.CancelAsync();
//             _cts.Dispose();
//
//             foreach (var session in _sessions)
//             {
//                 session.Complete();
//             }
//
//             while (_fileUpdatedEvents.Reader.TryRead(out _)) { }
//         }
//     }
//
//     private sealed class ObserverSession(
//         FileSystemOpenApiDefinitionStorage storage,
//         IObserver<OpenApiDefinitionStorageEventArgs> observer
//     ) : IDisposable
//     {
//         public void Notify(OpenApiDefinitionStorageEventArgs args)
//         {
//             try
//             {
//                 observer.OnNext(args);
//             }
//             catch (Exception ex)
//             {
//                 observer.OnError(ex);
//             }
//         }
//
//         public void Complete()
//         {
//             try
//             {
//                 observer.OnCompleted();
//             }
//             catch
//             {
//                 // We do not want to throw an exception if the observer
//                 // throws an exception on completion.
//             }
//         }
//
//         public void Dispose() => storage.Unsubscribe(this);
//     }
// }