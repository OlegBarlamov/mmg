using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using BoardPlatform.Data;
using BoardPlatform.Server.Data;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace BoardPlatform.Server.Services
{
    public interface IBoardSocketsWorker
    {
        IToken BoardToken { get; }
        Task HandleNewConnectionAsync(ClientInfo clientInfo, WebSocket clientSocket, CancellationToken cancellationToken);
    }
    
    public class DefaultBoardSocketsWorker : IBoardSocketsWorker
    {
        public IToken BoardToken { get; }
        public ILogger<DefaultBoardSocketsWorker> Logger { get; }

        [NotNull] private IWebSocketConnectionsService WebSocketConnectionsService { get; }
        
        private readonly ConcurrentDictionary<ClientInfo, IWebSocketConnection> _connections = new ConcurrentDictionary<ClientInfo, IWebSocketConnection>();

        public DefaultBoardSocketsWorker([NotNull] IWebSocketConnectionsService webSocketConnectionsService,
            [NotNull] IToken boardToken,
            [NotNull] ILogger<DefaultBoardSocketsWorker> logger)
        {
            WebSocketConnectionsService = webSocketConnectionsService ?? throw new ArgumentNullException(nameof(webSocketConnectionsService));
            BoardToken = boardToken ?? throw new ArgumentNullException(nameof(boardToken));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public Task HandleNewConnectionAsync(ClientInfo clientInfo, WebSocket clientSocket, CancellationToken cancellationToken)
        {
            var newConnection = _connections.AddOrUpdate(clientInfo, info => WebSocketConnectionsService.Create(clientSocket),
                (info, oldConnection) =>
                {
                    using (oldConnection)
                    {
                        if (oldConnection.IsRunning)
                            oldConnection.Close();
                    }

                    return WebSocketConnectionsService.Create(clientSocket);
                });

            newConnection.ConnectionLost += () =>
            {
                if (_connections.TryRemove(clientInfo, out var connection))
                {
                    connection.MessageReceived -= ConnectionOnMessageReceived;
                    connection.Dispose();
                }
            };
            
            return newConnection.Establish(cancellationToken);
        }

        private void ConnectionOnMessageReceived(IWsClientToServerMessage message)
        {
            Logger.LogWarning("Message received!!!");
        }
    }

    public interface IBoardSocketsWorkersManager
    {
        IBoardSocketsWorker GetWorker(IToken boardToken);
    }

    class BoardSocketsWorkersManager : IBoardSocketsWorkersManager
    {
        private IWebSocketConnectionsService WebSocketConnectionsService { get; }
        public ILogger<DefaultBoardSocketsWorker> Logger { get; }

        private readonly ConcurrentDictionary<IToken, IBoardSocketsWorker> _workers = new ConcurrentDictionary<IToken, IBoardSocketsWorker>();

        public BoardSocketsWorkersManager([NotNull] IWebSocketConnectionsService webSocketConnectionsService,
            [NotNull] ILogger<DefaultBoardSocketsWorker> logger)
        {
            WebSocketConnectionsService = webSocketConnectionsService ?? throw new ArgumentNullException(nameof(webSocketConnectionsService));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public IBoardSocketsWorker GetWorker(IToken boardToken)
        {
            return _workers.GetOrAdd(boardToken,
                (token, service) => new DefaultBoardSocketsWorker(service, token, Logger), WebSocketConnectionsService);
        }
    }
}