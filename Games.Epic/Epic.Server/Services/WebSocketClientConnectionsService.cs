using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Epic.Core;
using Epic.Core.Objects.ClientConnection;
using Epic.Core.Services.Connection;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Epic.Server.Services
{
    [UsedImplicitly]
    internal class WebSocketClientConnectionsService : IClientConnectionsService<WebSocket>
    {
        public ILoggerFactory LoggerFactory { get; }

        public WebSocketClientConnectionsService([NotNull] ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }
        public IClientConnection CreateConnection(WebSocket webSocket)
        {
            return new WebSocketClientConnection(webSocket, Guid.NewGuid(), LoggerFactory);
        }
    }

    internal class WebSocketClientConnection : IClientConnection
    {
        public event Action<string> OnMessageReceived;
        
        public Guid ConnectionId { get; }
        public bool IsActive { get; private set; }
        
        private WebSocket WebSocket { get; }
        private ILogger Logger { get; }
        
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public WebSocketClientConnection([NotNull] WebSocket webSocket, Guid connectionId, [NotNull] ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            WebSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
            ConnectionId = connectionId;
            Logger = loggerFactory.CreateLogger<WebSocketClientConnection>();
        }

        public void Dispose()
        {
            if (IsActive)
            {
                _cancellationTokenSource.Cancel();
                CloseAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        Logger.LogError(t.Exception, "Error while disposing websocket connection");
                });
            }
            _cancellationTokenSource.Dispose();
        }

        public Task SendMessageAsync(string message)
        {
            if (!IsActive)
                throw new InvalidOperationException("The client connection is not active.");
            
            var responseMessage = Encoding.UTF8.GetBytes(message);
            return WebSocket.SendAsync(
                new ArraySegment<byte>(responseMessage, 0, responseMessage.Length),
                WebSocketMessageType.Text,
                true,
                _cancellationTokenSource.Token);
        }

        public async Task ListenMessagesAsync()
        {
            IsActive = true;
            var buffer = new byte[1024 * 4];
            var result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);
            while (!result.CloseStatus.HasValue)
            {
                var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

                try
                {
                    OnMessageReceived?.Invoke(receivedMessage);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, $"Error while processing client-side message {receivedMessage}");
                }
                
                result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);
            }

            IsActive = false;
            await CloseAsyncInternal(result.CloseStatus.Value, result.CloseStatusDescription);
        }

        public Task CloseAsync()
        {
            if (IsActive)
            {
                IsActive = false;
                _cancellationTokenSource.Cancel();
                return CloseAsyncInternal();
            }
            return Task.CompletedTask;
        }
        
        private Task CloseAsyncInternal(WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure, string statusDescription = "Closing")
        {
            if (WebSocket.State == WebSocketState.Open || WebSocket.State == WebSocketState.CloseReceived)
            {
                return WebSocket.CloseAsync(closeStatus, statusDescription, CancellationToken.None);
            }
            return Task.CompletedTask;
        }
    }
}