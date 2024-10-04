using System;
using System.Threading.Tasks;
using Epic.Core.ClientMessages;
using Epic.Core.Objects.Battle;
using Epic.Core.Objects.ClientConnection;
using Epic.Core.ServerMessages;
using JetBrains.Annotations;

namespace Epic.Core.Objects.BattleClientConnection
{
    public interface IBattleClientConnection : IDisposable
    {
        event Action<IClientBattleMessage> MessageReceived;
        Guid ConnectionId { get; }
        
        IBattleObject BattleObject { get; }
        
        Task SendMessageAsync([NotNull] IServerBattleMessage message);
    }

    internal class BattleClientConnection : IBattleClientConnection
    {
        public event Action<IClientBattleMessage> MessageReceived;
        public IClientConnection ClientConnection { get; }
        
        public Guid ConnectionId => ClientConnection.ConnectionId;
        public IBattleObject BattleObject { get; }
        public IClientMessagesParserService ClientMessagesParserService { get; }

        public BattleClientConnection([NotNull] IClientConnection clientConnection, [NotNull] IBattleObject battleObject,
            [NotNull] IClientMessagesParserService clientMessagesParserService)
        {
            ClientConnection = clientConnection ?? throw new ArgumentNullException(nameof(clientConnection));
            BattleObject = battleObject ?? throw new ArgumentNullException(nameof(battleObject));
            ClientMessagesParserService = clientMessagesParserService ?? throw new ArgumentNullException(nameof(clientMessagesParserService));
            ClientConnection.OnMessageReceived += ClientConnectionOnOnMessageReceived;
        }

        private void ClientConnectionOnOnMessageReceived(string message)
        {
            if (ClientMessagesParserService.ParseSafe(message, out var parsedMessage))
            {
                MessageReceived?.Invoke(parsedMessage);
            }
        }

        public Task SendMessageAsync(IServerBattleMessage message)
        {
            return ClientConnection.SendMessageAsync(message.ToStringMessage());
        }

        public void Dispose()
        {
            ClientConnection.OnMessageReceived -= ClientConnectionOnOnMessageReceived;
            MessageReceived = null;
        }
    }
}