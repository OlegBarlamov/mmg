using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Epic.Core.Objects.Battle;
using Epic.Core.Objects.BattleClientConnection;
using Epic.Core.Objects.BattleGameManager;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Epic.Core
{
    [UsedImplicitly]
    public class DefaultBattleGameManagersService : IBattleGameManagersService
    {
        public IBattlesCacheService BattlesCacheService { get; }
        public ILoggerFactory LoggerFactory { get; }
        public IBattleUnitsService BattleUnitsService { get; }
        public IBattlesService BattlesService { get; }

        private readonly ConcurrentDictionary<Guid, BattleGameManager> _battleGameManagers = new ConcurrentDictionary<Guid, BattleGameManager>();

        public DefaultBattleGameManagersService(
            [NotNull] IBattlesCacheService battlesCacheService,
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IBattleUnitsService battleUnitsService,
            [NotNull] IBattlesService battlesService)
        {
            BattlesCacheService = battlesCacheService ?? throw new ArgumentNullException(nameof(battlesCacheService));
            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            BattleUnitsService = battleUnitsService ?? throw new ArgumentNullException(nameof(battleUnitsService));
            BattlesService = battlesService ?? throw new ArgumentNullException(nameof(battlesService));
        }
        
        public Task<IBattleGameManager> GetBattleGameManager(IBattleClientConnection clientConnection)
        {
            var battleObject = (MutableBattleObject)clientConnection.BattleObject;
            if (BattlesCacheService.FindBattleById(battleObject.Id) == null)
            {
                BattlesCacheService.AddBattle(battleObject);
            }
            
            var manager = CreateBattleGameManager(battleObject);
            manager.AddClient(clientConnection);
            if (!manager.IsBattlePlaying())
                manager.PlayBattle();
            
            return Task.FromResult((IBattleGameManager)manager);
        }

        public Task<IBattleGameManager> RemoveClientConnection(IBattleClientConnection clientConnection)
        {
            if (!_battleGameManagers.TryGetValue(clientConnection.BattleObject.Id, out var manager)) 
                throw new InvalidOperationException($"The battle game manager was not found for battle {clientConnection.BattleObject.Id}"); 
                    
            manager.RemoveClient(clientConnection);
            if (manager.GetClientsCount() == 0)
            {
                BattlesCacheService.ReleaseBattle(clientConnection.BattleObject.Id);
                _battleGameManagers.TryRemove(manager.BattleObject.Id, out _);
                manager.SuspendBattle();
                manager.Dispose();
            }
            
            return Task.FromResult((IBattleGameManager)manager);
        }

        private BattleGameManager CreateBattleGameManager(MutableBattleObject battleObject)
        {
            return _battleGameManagers.GetOrAdd(battleObject.Id, id => new BattleGameManager(
                battleObject,
                LoggerFactory,
                BattleUnitsService,
                BattlesService));
        }
    }
}