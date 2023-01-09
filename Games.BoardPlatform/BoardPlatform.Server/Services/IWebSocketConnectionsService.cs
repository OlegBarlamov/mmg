using System;
using System.Net.WebSockets;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace BoardPlatform.Server.Services
{
    public interface IWebSocketConnectionsService
    {
        IWebSocketConnection Create(WebSocket webSocket);
    }

    class WebSocketConnectionsService : IWebSocketConnectionsService
    {
        public ILogger<WebSocketConnection> Logger { get; }

        public WebSocketConnectionsService([NotNull] ILogger<WebSocketConnection> logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public IWebSocketConnection Create(WebSocket webSocket)
        {
            return new WebSocketConnection(Logger, webSocket);
        }
    }
}