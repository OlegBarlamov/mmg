using System;
using Epic.Core.Objects.Battle;
using Epic.Core.Objects.BattleClientConnection;
using Epic.Core.Objects.ClientConnection;
using JetBrains.Annotations;

namespace Epic.Core
{
    public interface IBattleConnectionsService
    {
        IBattleClientConnection CreateConnection(IClientConnection clientConnection, IBattleObject battleObject);
    }

    public class BattleConnectionsService : IBattleConnectionsService
    {
        public IClientMessagesParserService ClientMessagesParserService { get; }

        public BattleConnectionsService([NotNull] IClientMessagesParserService clientMessagesParserService)
        {
            ClientMessagesParserService = clientMessagesParserService ?? throw new ArgumentNullException(nameof(clientMessagesParserService));
        }
        
        public IBattleClientConnection CreateConnection(IClientConnection clientConnection, IBattleObject battleObject)
        {
            return new BattleClientConnection(clientConnection, battleObject, ClientMessagesParserService);
        }
    }
}