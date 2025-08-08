using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.Units;
using Epic.Core.Services.UnitsContainers;
using Epic.Data.BattleDefinitions;
using Epic.Data.Players;
using JetBrains.Annotations;

namespace Epic.Core.Services.BattleDefinitions
{
    [UsedImplicitly]
    public class DefaultBattleDefinitionsService : IBattleDefinitionsService
    {
        public IBattleDefinitionsRepository BattleDefinitionsRepository { get; }
        public IGlobalUnitsService GlobalUnitsService { get; }
        public IUnitsContainersService UnitsContainersService { get; }
        public IPlayersRepository PlayersRepository { get; }

        public DefaultBattleDefinitionsService(
            [NotNull] IBattleDefinitionsRepository battleDefinitionsRepository,
            [NotNull] IGlobalUnitsService globalUnitsService,
            [NotNull] IUnitsContainersService unitsContainersService,
            [NotNull] IPlayersRepository playersRepository)
        {
            BattleDefinitionsRepository = battleDefinitionsRepository ?? throw new ArgumentNullException(nameof(battleDefinitionsRepository));
            GlobalUnitsService = globalUnitsService ?? throw new ArgumentNullException(nameof(globalUnitsService));
            UnitsContainersService = unitsContainersService ?? throw new ArgumentNullException(nameof(unitsContainersService));
            PlayersRepository = playersRepository ?? throw new ArgumentNullException(nameof(playersRepository));
        }

        public Task<int> GetBattlesCountForPlayer(Guid playerId)
        {
            return BattleDefinitionsRepository.CountBattles(playerId);
        }

        public async Task<IReadOnlyCollection<IBattleDefinitionObject>> GetNotExpiredActiveBattleDefinitionsByPlayerAsync(Guid playerId)
        {
            var playerEntity = await PlayersRepository.GetById(playerId);
            var entities = await BattleDefinitionsRepository.GetActiveBattleDefinitionsByPlayer(playerId, playerEntity.Day);
            var battleDefinitions = entities.Select(MutableBattleDefinitionObject.FromEntity).ToArray();
            await Task.WhenAll(battleDefinitions.Select(x => FillBattleDefinitionObject(x)));
            return battleDefinitions;
        }

        public async Task<IBattleDefinitionObject> GetBattleDefinitionById(Guid battleDefinitionId)
        {
            var entity = await BattleDefinitionsRepository.GetById(battleDefinitionId);
            var battleDefinition = MutableBattleDefinitionObject.FromEntity(entity);
            await FillBattleDefinitionObject(battleDefinition);
            return battleDefinition;
        }

        public async Task<IBattleDefinitionObject> GetBattleDefinitionByPlayerAndId(Guid playerId, Guid battleDefinitionId)
        {
            var battleDefinition =
                await BattleDefinitionsRepository.GetByPlayerAndId(playerId, battleDefinitionId);
            var battleDefinitionObject = MutableBattleDefinitionObject.FromEntity(battleDefinition);
            await FillBattleDefinitionObject(battleDefinitionObject);
            return battleDefinitionObject;
        }

        public Task<IBattleDefinitionObject> CreateBattleDefinition(
            Guid playerId,
            int width,
            int height,
            int expireAtDay,
            int rewardVisibility,
            int guardVisibility,
            Guid? containerId = null)
        {
            return CreateBattleDefinitionInternal(width, height, expireAtDay, rewardVisibility, guardVisibility, containerId, playerId);
        }

        public Task<IBattleDefinitionObject> CreateBattleDefinition(int width, int height)
        {
            return CreateBattleDefinitionInternal(width, height, 0, 0, Int32.MaxValue);
        }

        private async Task<IBattleDefinitionObject> CreateBattleDefinitionInternal(
            int width,
            int height,
            int expireAtDay,
            int rewardVisibility,
            int guardVisibility,
            Guid? containerId = null,
            Guid? playerId = null)
        {
            var container = containerId.HasValue 
                ? await UnitsContainersService.GetById(containerId.Value)
                : await UnitsContainersService.Create(height, Guid.Empty);
            
            var fields = new BattleDefinitionEntityFields
            {
                Height = height,
                Width = width,
                ContainerId = container.Id,
                Finished = false,
                CreatedAt = DateTime.Now,
                ExpireAtDay = expireAtDay,
                ExpireAt = null,
                RewardVisibility = rewardVisibility,
                GuardVisibility = guardVisibility,
            };
            var entity = playerId.HasValue
                ? await BattleDefinitionsRepository.Create(playerId.Value, fields)
                : await BattleDefinitionsRepository.Create(fields);
            
            var battleDefinitionObject = MutableBattleDefinitionObject.FromEntity(entity);
            await FillBattleDefinitionObject(battleDefinitionObject, container);
            return battleDefinitionObject;
        }

        public Task SetFinished(Guid battleDefinitionId)
        {
            return BattleDefinitionsRepository.SetFinished(battleDefinitionId);
        }

        private async Task FillBattleDefinitionObject(MutableBattleDefinitionObject battleDefinitionObject, IUnitsContainerObject unitsContainerObject = null)
        {

            battleDefinitionObject.Units =
                await GlobalUnitsService.GetAliveUnitsByContainerId(battleDefinitionObject.ContainerId);
            battleDefinitionObject.UnitsContainerObject = unitsContainerObject ??
                await UnitsContainersService.GetById(battleDefinitionObject.ContainerId);
        }
    }
}