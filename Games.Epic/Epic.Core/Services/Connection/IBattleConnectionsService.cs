using System;
using Epic.Core.Objects.ClientConnection;
using Epic.Core.Services.Battles;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Epic.Core.Services.Connection
{
    public interface IBattleConnectionsService
    {
        IBattleClientConnection CreateConnection(IClientConnection clientConnection, IBattleObject battleObject);
    }

    [UsedImplicitly]
    public class BattleConnectionsService : IBattleConnectionsService
    {
        public IClientMessagesParserService ClientMessagesParserService { get; }
        [NotNull] public ILoggerFactory LoggerFactory { get; }

        public BattleConnectionsService([NotNull] IClientMessagesParserService clientMessagesParserService, [NotNull] ILoggerFactory loggerFactory)
        {
            ClientMessagesParserService = clientMessagesParserService ?? throw new ArgumentNullException(nameof(clientMessagesParserService));
            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }
        
        public IBattleClientConnection CreateConnection(IClientConnection clientConnection, IBattleObject battleObject)
        {
            return new BattleClientConnection(clientConnection, battleObject, ClientMessagesParserService, LoggerFactory);
        }
    }
}