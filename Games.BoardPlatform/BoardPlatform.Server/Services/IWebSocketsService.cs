using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BoardPlatform.Server.Services
{
    public interface IWebSocketsService
    {
        event Action<IWsClientToServerMessage> MessageReceived;

        Task HandleConnectionAsync(WebSocket webSocket, CancellationToken cancellationToken);

        void Broadcast(IWsServerToClientMessage message);
    }

    public class WebSocketsService : IWebSocketsService
    {
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

        public event Action<IWsClientToServerMessage> MessageReceived;

        private readonly ConcurrentQueue<IWsServerToClientMessage> _messagesToSend = new ConcurrentQueue<IWsServerToClientMessage>();

        public Task HandleConnectionAsync(WebSocket webSocket, CancellationToken cancellationToken)
        {
            var state = new WorkingFunctionsState(webSocket, cancellationToken);
            return Task.WhenAll(
                Task.Factory.StartNew(ReadFunction, state, cancellationToken, TaskCreationOptions.LongRunning,
                    TaskScheduler.Default),
                Task.Factory.StartNew(WriteFunction, state, cancellationToken, TaskCreationOptions.LongRunning,
                    TaskScheduler.Default)
            );
        }

        public void Broadcast(IWsServerToClientMessage message)
        {
            _messagesToSend.Enqueue(message);
        }

        private void ReadFunction(object arg)
        {
            var state = (WorkingFunctionsState) arg;

            var memoryStream = new MemoryStream();
            var buffer = new byte[4 * 1024];
            
            while (!state.CancellationToken.IsCancellationRequested)
            {
                WebSocketReceiveResult receive;
                do
                {
                    receive = state.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), state.CancellationToken).Result;
                    memoryStream.Write(buffer, 0, receive.Count);
                } while (!receive.EndOfMessage);

                var message = JsonSerializer.Deserialize<IWsClientToServerMessage>(memoryStream);
                MessageReceived?.Invoke(message);
            }
        }
        
        private void WriteFunction(object arg)
        {
            var state = (WorkingFunctionsState) arg;
            
            var messageStream = new MemoryStream();
            var bufferSize = 4 * 1024;
            var buffer = new byte[bufferSize];

            while (!state.CancellationToken.IsCancellationRequested)
            {
                while (_messagesToSend.TryDequeue(out var message))
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
                
                Thread.Sleep(100);
            }
        }
    }
}