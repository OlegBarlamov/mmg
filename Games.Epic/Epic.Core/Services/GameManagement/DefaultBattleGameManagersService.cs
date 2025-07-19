using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Epic.Core.Objects.Battle;
using Epic.Core.Objects.BattleClientConnection;
using Epic.Core.Services.Battles;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Epic.Core.Services.GameManagement
{
    [UsedImplicitly]
    public class DefaultBattleGameManagersService : IBattleGameManagersService
    {
        private IBattlesCacheService BattlesCacheService { get; }
        private ILoggerFactory LoggerFactory { get; }
        private IBattleLogicFactory BattleLogicFactory { get; }

        private readonly ConcurrentDictionary<Guid, BattleGameManager> _battleGameManagers = new ConcurrentDictionary<Guid, BattleGameManager>();

        public DefaultBattleGameManagersService(
            [NotNull] IBattlesCacheService battlesCacheService,
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IBattleLogicFactory battleLogicFactory)
        {
            BattlesCacheService = battlesCacheService ?? throw new ArgumentNullException(nameof(battlesCacheService));
            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            BattleLogicFactory = battleLogicFactory ?? throw new ArgumentNullException(nameof(battleLogicFactory));
        }
        
        public Task<IBattleGameManager> GetBattleGameManager(IBattleClientConnection clientConnection)
        {
            var battleObject = (MutableBattleObject)clientConnection.BattleObject;
            BattlesCacheService.AddIfAbsent(battleObject);
            
            var manager = CreateBattleGameManager(battleObject);
            manager.AddClient(clientConnection);
            
            return Task.FromResult((IBattleGameManager)manager);
        }

        public Task RemoveClientConnection(IBattleClientConnection clientConnection)
        {
            // Battle might be already removed due to finishing
            if (_battleGameManagers.TryGetValue(clientConnection.BattleObject.Id, out var manager))
            {
                manager.RemoveClient(clientConnection);
            }
            return Task.CompletedTask;
        }

        private BattleGameManager CreateBattleGameManager(MutableBattleObject battleObject)
        {
            return _battleGameManagers.GetOrAdd(battleObject.Id, id =>
            {
                var newInstance = new BattleGameManager(
                    battleObject,
                    LoggerFactory,
                    BattleLogicFactory);
                newInstance.Finished += GameManagerOnFinished;
                return newInstance;
            });
        }

        private void GameManagerOnFinished(IBattleGameManager gameManager)
        {
            // TODO So far, game manager persists until gets finished
            _battleGameManagers.TryRemove(gameManager.BattleId, out _);
            BattlesCacheService.ReleaseBattle(gameManager.BattleId);
            gameManager.Finished -= GameManagerOnFinished;
            gameManager.Dispose();
        }
    }
}