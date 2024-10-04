using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Epic.Core.Objects.Battle;
using Epic.Core.Objects.BattleClientConnection;
using Epic.Core.Objects.BattleGameManager;
using JetBrains.Annotations;

namespace Epic.Core
{
    [UsedImplicitly]
    public class DefaultBattleGameManagersService : IBattleGameManagersService
    {
        public IBattlesCacheService BattlesCacheService { get; }
        
        private readonly ConcurrentDictionary<Guid, BattleGameManager> _battleGameManagers = new ConcurrentDictionary<Guid, BattleGameManager>();

        public DefaultBattleGameManagersService([NotNull] IBattlesCacheService battlesCacheService)
        {
            BattlesCacheService = battlesCacheService ?? throw new ArgumentNullException(nameof(battlesCacheService));
        }
        
        public Task<IBattleGameManager> GetBattleGameManager(IBattleClientConnection clientConnection)
        {
            var battleObject = clientConnection.BattleObject;
            if (BattlesCacheService.FindBattleById(battleObject.Id) == null)
            {
                BattlesCacheService.AddBattle(battleObject);
            }
            
            var manager = CreateBattleGameManager(battleObject);
            manager.AddClient(clientConnection);
            
            return Task.FromResult((IBattleGameManager)manager);
        }

        public Task<IBattleGameManager> RemoveClientConnection(IBattleClientConnection clientConnection)
        {
            if (!_battleGameManagers.TryGetValue(clientConnection.BattleObject.Id, out var manager)) 
                throw new InvalidOperationException($"The battle game manager was not found for battle {clientConnection.BattleObject.Id}"); 
                    
            manager.RemoveClient(clientConnection);
            if (manager.GetClientsCount() == 0)
            {
                BattlesCacheService.ReleaseBattle(clientConnection.BattleObject);
                _battleGameManagers.TryRemove(manager.BattleObject.Id, out _);
                manager.Dispose();
            }
            
            return Task.FromResult((IBattleGameManager)manager);
        }

        private BattleGameManager CreateBattleGameManager(IBattleObject battleObject)
        {
            return _battleGameManagers.GetOrAdd(battleObject.Id, id => new BattleGameManager(battleObject));
        }
    }
}