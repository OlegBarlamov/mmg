using System;
using System.Text.Json;
using System.Threading.Tasks;
using Epic.Core.ClientMessages;
using Epic.Core.Objects.Battle;
using Epic.Core.Objects.ClientConnection;
using Epic.Core.ServerMessages;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Epic.Core.Objects.BattleClientConnection
{
    public interface IBattleClientConnection : IDisposable
    {
        event Action<IBattleClientConnection, IClientBattleMessage> MessageReceived;
        Guid ConnectionId { get; }
        public IBattleObject BattleObject { get; }
        
        Task SendMessageAsync([NotNull] IServerBattleMessage message);
    }

    internal class BattleClientConnection : IBattleClientConnection
    {
        public event Action<IBattleClientConnection, IClientBattleMessage> MessageReceived;
        public IClientConnection ClientConnection { get; }
        
        public Guid ConnectionId => ClientConnection.ConnectionId;
        public IBattleObject BattleObject { get; }
        public IClientMessagesParserService ClientMessagesParserService { get; }
        
        private ILogger Logger { get; }

        public BattleClientConnection(
            [NotNull] IClientConnection clientConnection,
            [NotNull] IBattleObject battleObject,
            [NotNull] IClientMessagesParserService clientMessagesParserService,
            [NotNull] ILoggerFactory loggerFactory
            )
        {
            ClientConnection = clientConnection ?? throw new ArgumentNullException(nameof(clientConnection));
            BattleObject = battleObject ?? throw new ArgumentNullException(nameof(battleObject));
            ClientMessagesParserService = clientMessagesParserService ?? throw new ArgumentNullException(nameof(clientMessagesParserService));
            ClientConnection.OnMessageReceived += ClientConnectionOnOnMessageReceived;
            Logger = loggerFactory.CreateLogger<BattleClientConnection>();
        }

        private void ClientConnectionOnOnMessageReceived(string message)
        {
            if (ClientMessagesParserService.ParseSafe(message, out var parsedMessage))
            {
                MessageReceived?.Invoke(this, parsedMessage);
            }
        }

        public Task SendMessageAsync(IServerBattleMessage message)
        {
            Logger.LogInformation($"Sending message to {ConnectionId}: {message.Command}");
            
            var json = JsonSerializer.Serialize((object)message, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }
            );
            return ClientConnection.SendMessageAsync(json);
        }

        public void Dispose()
        {
            ClientConnection.OnMessageReceived -= ClientConnectionOnOnMessageReceived;
            MessageReceived = null;
        }
    }
}