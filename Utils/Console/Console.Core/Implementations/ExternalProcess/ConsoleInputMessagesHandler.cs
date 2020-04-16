using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Console.Core.Implementations.ExternalProcess.ProcessMessages;
using Console.Core.Implementations.ExternalProcess.ProcessMessages.Messages;
using Console.Core.Models;
using JetBrains.Annotations;

namespace Console.Core.Implementations.ExternalProcess
{
    public class ConsoleInputMessagesHandler : IConsoleMessagesProvider, IDisposable
    {
        public event Action NewMessages;
        
        // ReSharper disable once InconsistentlySynchronizedField
        public bool IsQueueEmpty => _receivedMessages.Count == 0;
        
        private IConsoleController CurrentProcessController { get; }
        private IConsoleProcessMessageFormatter ConsoleProcessMessageFormatter { get; }

        private bool _started;
        private bool _disposed;
        private BinaryReader _stdReader; 
        
        private readonly CancellationTokenSource _listenCancellationTokenSource = new CancellationTokenSource();
        private readonly Queue<IConsoleMessage> _receivedMessages = new Queue<IConsoleMessage>(); 
        private readonly object _messagesQueueLocker = new object();
        private readonly GenericHandler<IConsoleProcessMessage> _commandsHandler = new GenericHandler<IConsoleProcessMessage>();

        public ConsoleInputMessagesHandler([NotNull] IConsoleController currentProcessController,
            [NotNull] IConsoleProcessMessageFormatter consoleProcessMessageFormatter)
        {
            CurrentProcessController = currentProcessController ?? throw new ArgumentNullException(nameof(currentProcessController));
            ConsoleProcessMessageFormatter = consoleProcessMessageFormatter ?? throw new ArgumentNullException(nameof(consoleProcessMessageFormatter));
            
            _commandsHandler.Add<ShowMessage>(message => CurrentProcessController.Show());
            _commandsHandler.Add<HideMessage>(message => CurrentProcessController.Hide());
            _commandsHandler.Add<ClearCurrentMessage>(message => CurrentProcessController.ClearCurrent());
            _commandsHandler.Add<ClearAllMessage>(message => CurrentProcessController.ClearAll());
            _commandsHandler.Add<NewMessagesConsoleProcessMessage>(message => OnStdInputNewMessages(message.Messages));
        }

        public void StartListen()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ConsoleInputMessagesHandler));
            if (_started) throw new ConsoleException("Listening std input already started");

            _started = true;
            _stdReader = new BinaryReader(System.Console.OpenStandardInput(), System.Console.InputEncoding);
            Task.Factory.StartNew(ListeningFunc, _listenCancellationTokenSource.Token,
                TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }

        public void SendUserCommand([NotNull] string userCommand)
        {
            if (userCommand == null) throw new ArgumentNullException(nameof(userCommand));
            
            System.Console.Out.WriteLine(userCommand);
        }
        
        public IConsoleMessage Pop()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ConsoleInputMessagesHandler));
            
            lock (_messagesQueueLocker)
            {
                return _receivedMessages.Dequeue();
            }
        }

        public void Dispose()
        {
            NewMessages = null;
            
            using (_listenCancellationTokenSource)
            {
                _listenCancellationTokenSource.Cancel();
            }

            _disposed = true;
            lock (_messagesQueueLocker)
            {
                _receivedMessages.Clear();                
            }
            
            _stdReader?.Dispose();
            _commandsHandler.Clear();
        }

        private void OnStdInputNewMessages(IReadOnlyCollection<IConsoleMessage> messages)
        {
            lock (_messagesQueueLocker)
            {
                foreach (var message in messages)
                {
                    _receivedMessages.Enqueue(message);
                }
            }
            
            NewMessages?.Invoke();
        }

        private async void ListeningFunc()
        {
            var token = _listenCancellationTokenSource.Token;
            
            while (true)
            {
                try
                {
                    var command = await ReadCommand(token);
                    
                    token.ThrowIfCancellationRequested();
                    if (command != null)
                        HandleCommand(command, token);
                }
                catch (OperationCanceledException e)
                {
                    //ignore
                }
                catch (Exception e)
                {
                    //ignore
                }
            }
        }

        private async Task<IConsoleProcessMessage> ReadCommand(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var dataSize = await ReadHeader(token);
            if (dataSize < 1)
                return null;

            var data = await ReadBytes(dataSize, token);
            token.ThrowIfCancellationRequested();
            return ConsoleProcessMessageFormatter.Deserialize(data);
        }

        private Task<int> ReadHeader(CancellationToken token)
        {
            var dataSize = _stdReader.ReadInt32();
            token.ThrowIfCancellationRequested();
            
            return Task.FromResult(dataSize);
        }

        private Task<byte[]> ReadBytes(int size, CancellationToken token)
        {
            var data = _stdReader.ReadBytes(size);
            token.ThrowIfCancellationRequested();
            return Task.FromResult(data);
        }

        private void HandleCommand(IConsoleProcessMessage command, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            try
            {
                _commandsHandler.Handle(command);
            }
            catch (KeyNotFoundException e)
            {
                //TODO command unknown
            }
        }
    }
}