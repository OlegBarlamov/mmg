using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BoardPlatform.Server.Data;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BoardPlatform.Server
{
    public class WebSocketConnection : IWebSocketConnection
    {
        public bool IsRunning => _isRunning;
        public WsConnectionStatus Status { get; private set; }

        public ClientAppInfo ClientAppInfo { get; private set; }
        public event Action ConnectionLost;
        public event Action<IWsClientToServerMessage> MessageReceived;

        private ILogger<WebSocketConnection> Logger { get; }
        private WebSocket Socket { get; }
        
        private readonly object _connectionLocker = new object();
        private bool _isRunning;
        private Task _closingTask;
        
        private readonly ConcurrentQueue<IWsServerToClientMessage> _messagesToSend = new ConcurrentQueue<IWsServerToClientMessage>();

        public WebSocketConnection([NotNull] ILogger<WebSocketConnection> logger, [NotNull] WebSocket socket)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Socket = socket ?? throw new ArgumentNullException(nameof(socket));
        }
        
        public void Dispose()
        {
            ConnectionLost = null;
            MessageReceived = null;
            
            if (_isRunning)
            {
                if (Status == WsConnectionStatus.Closing)
                {
                    _closingTask.ContinueWith(x => { Socket.Dispose(); });
                }
                else
                {
                    CloseInternal("Server connection disposed", true);
                }
            }

            _closingTask = null;
            _messagesToSend.Clear();
        }

        public Task Establish(CancellationToken cancellationToken)
        {
            lock (_connectionLocker)
            {
                if (_isRunning)
                    throw new Exception("Connection already established");

                _isRunning = true;
            }

            Status = WsConnectionStatus.Connecting;
            var state = new WorkingFunctionsState(Socket, cancellationToken);
            var workingTasks = Task.WhenAll(
                Task.Factory.StartNew(ReadFunction, state, cancellationToken, TaskCreationOptions.LongRunning,
                    TaskScheduler.Default),
                Task.Factory.StartNew(WriteFunction, state, cancellationToken, TaskCreationOptions.LongRunning,
                    TaskScheduler.Default)
            );

            var messageId = Guid.NewGuid().GetHashCode();
            WaitForCommandAsync(WsClientToServerCommand.ConnectedHandshake, cancellationToken).ContinueWith(task =>
            {
                var handshakeMessage = task.Result;
                var handshakeData = (WsClientToServerHandshakeMessagePayload) handshakeMessage.Payload;
                ClientAppInfo = handshakeData.ClientAppInfo;
                Status = WsConnectionStatus.Connected;
            }, cancellationToken);
            
            SendMessage(new WsServerToClientConnectedHandshakeMessage(messageId));

            return workingTasks.ContinueWith(ConnectionClosed, cancellationToken);
        }

        public void Close()
        {
            CloseInternal("new connection from the same client", false);
        }

        public void SendMessage(IWsServerToClientMessage message)
        {
            _messagesToSend.Enqueue(message);
        }
        
        public Task<IWsClientToServerMessage> WaitForCommandAsync(WsClientToServerCommand command, CancellationToken cancellationToken)
        {
            var taskCompletionSource = new TaskCompletionSource<IWsClientToServerMessage>(); 
            
            void OnMessageReceived(IWsClientToServerMessage message)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    MessageReceived -= OnMessageReceived;
                    taskCompletionSource.SetCanceled(cancellationToken);
                    return;
                }
                
                if (message.Command == command)
                {
                    MessageReceived -= OnMessageReceived;
                    taskCompletionSource.SetResult(message);
                }
            }
            
            MessageReceived += OnMessageReceived;

            return taskCompletionSource.Task;
        }

        private void CloseInternal(string message, bool disposeSocket)
        {
            Status = WsConnectionStatus.Closing;
            _closingTask = Socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, message, CancellationToken.None).ContinueWith(
                x =>
                {
                    _isRunning = false;
                    Status = WsConnectionStatus.Closed;
                    if (disposeSocket)
                        Socket.Dispose();
                });
        }

        private void ConnectionClosed(Task connectionTask)
        {
            _isRunning = false;
            Status = WsConnectionStatus.Closed;
            
            ConnectionLost?.Invoke();
        }

        private void ReadFunction(object arg)
        {
            var state = (WorkingFunctionsState) arg;

            var memoryStream = new MemoryStream();
            var buffer = new byte[4 * 1024];

            while (!state.CancellationToken.IsCancellationRequested && _isRunning)
            {
                IWsClientToServerMessage message = null;
                try
                {
                    WebSocketReceiveResult receive;
                    do
                    {
                        receive = state.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), state.CancellationToken)
                            .Result;
                        memoryStream.Write(buffer, 0, receive.Count);
                    } while (!receive.EndOfMessage);
                    
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    message = JsonSerializer.Deserialize<IWsClientToServerMessage>(memoryStream, options);
                }
                catch (Exception e)
                {
                    Logger.LogError("Error in WS read: " + e);
                }

                if (message != null)
                {
                    try
                    {
                        MessageReceived?.Invoke(message);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError("Error in WS message {} handling: " + e, message);
                    }
                }
            }
        }
        
        private void WriteFunction(object arg)
        {
            var state = (WorkingFunctionsState) arg;
            
            var messageStream = new MemoryStream();
            var bufferSize = 4 * 1024;
            var buffer = new byte[bufferSize];

            while (!state.CancellationToken.IsCancellationRequested && _isRunning)
            {
                while (_messagesToSend.TryDequeue(out var message))
                {
                    try
                    {
                        var serializeOptions = new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            WriteIndented = true
                        };
                        JsonSerializer.Serialize(messageStream, message, serializeOptions);
                        messageStream.Seek(0, SeekOrigin.Begin);

                        int writtenBytesCount;
                        do
                        {
                            writtenBytesCount =
                                messageStream.Read(buffer, 0, bufferSize);
                        
                            if (writtenBytesCount > 0)
                            {
                                state.Socket.SendAsync(new ArraySegment<byte>(buffer, 0, writtenBytesCount),
                                    WebSocketMessageType.Text,
                                    messageStream.Position == messageStream.Length,
                                    state.CancellationToken);
                            }

                        } while (writtenBytesCount > 0);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError("Error in sending WS message {}: " + e, message);
                    }
                }
                
                Thread.Sleep(100);
            }
        }

        private class WorkingFunctionsState
        {
            public WebSocket Socket { get; }
            public CancellationToken CancellationToken { get; }
            
            public WorkingFunctionsState(WebSocket socket, CancellationToken cancellationToken)
            {
                Socket = socket;
                CancellationToken = cancellationToken;
            }
        }
    }
}