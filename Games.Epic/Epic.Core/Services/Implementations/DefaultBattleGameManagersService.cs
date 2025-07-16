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
        private IBattlesCacheService BattlesCacheService { get; }
        private ILoggerFactory LoggerFactory { get; }
        private IBattleUnitsService BattleUnitsService { get; }
        private IUserUnitsService UserUnitsService { get; }
        private IBattlesService BattlesService { get; }
        private IRewardsService RewardsService { get; }

        private readonly ConcurrentDictionary<Guid, BattleGameManager> _battleGameManagers = new ConcurrentDictionary<Guid, BattleGameManager>();

        public DefaultBattleGameManagersService(
            [NotNull] IBattlesCacheService battlesCacheService,
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IBattleUnitsService battleUnitsService,
            [NotNull] IUserUnitsService userUnitsService,
            [NotNull] IBattlesService battlesService,
            [NotNull] IRewardsService rewardsService)
        {
            BattlesCacheService = battlesCacheService ?? throw new ArgumentNullException(nameof(battlesCacheService));
            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            BattleUnitsService = battleUnitsService ?? throw new ArgumentNullException(nameof(battleUnitsService));
            UserUnitsService = userUnitsService ?? throw new ArgumentNullException(nameof(userUnitsService));
            BattlesService = battlesService ?? throw new ArgumentNullException(nameof(battlesService));
            RewardsService = rewardsService ?? throw new ArgumentNullException(nameof(rewardsService));
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
            
            return Task.FromResult((IBattleGameManager)manager);
        }

        public Task<IBattleGameManager> RemoveClientConnection(IBattleClientConnection clientConnection)
        {
            if (!_battleGameManagers.TryGetValue(clientConnection.BattleObject.Id, out var manager)) 
                throw new InvalidOperationException($"The battle game manager was not found for battle {clientConnection.BattleObject.Id}"); 
                    
            manager.RemoveClient(clientConnection);
            
            return Task.FromResult((IBattleGameManager)manager);
        }

        private BattleGameManager CreateBattleGameManager(MutableBattleObject battleObject)
        {
            var gameManager = _battleGameManagers.GetOrAdd(battleObject.Id, id => new BattleGameManager(
                battleObject,
                LoggerFactory,
                BattleUnitsService,
                UserUnitsService,
                BattlesService,
                RewardsService));

            gameManager.Finished += GameManagerOnFinished;
            
            return gameManager;
        }

        private void GameManagerOnFinished(IBattleGameManager gameManager)
        {
            gameManager.Dispose();
        }
    }
}