using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.Units;
using Epic.Core.Services.UnitsContainers;
using Epic.Data.BattleDefinitions;
using JetBrains.Annotations;

namespace Epic.Core.Services.BattleDefinitions
{
    [UsedImplicitly]
    public class DefaultBattleDefinitionsService : IBattleDefinitionsService
    {
        public IBattleDefinitionsRepository BattleDefinitionsRepository { get; }
        public IPlayerUnitsService PlayerUnitsService { get; }
        public IUnitsContainersService UnitsContainersService { get; }

        public DefaultBattleDefinitionsService(
            [NotNull] IBattleDefinitionsRepository battleDefinitionsRepository,
            [NotNull] IPlayerUnitsService playerUnitsService,
            [NotNull] IUnitsContainersService unitsContainersService)
        {
            BattleDefinitionsRepository = battleDefinitionsRepository ?? throw new ArgumentNullException(nameof(battleDefinitionsRepository));
            PlayerUnitsService = playerUnitsService ?? throw new ArgumentNullException(nameof(playerUnitsService));
            UnitsContainersService = unitsContainersService ?? throw new ArgumentNullException(nameof(unitsContainersService));
        }

        public Task<int> GetBattlesCountForPlayer(Guid playerId)
        {
            return BattleDefinitionsRepository.CountBattles(playerId);
        }

        public async Task<IReadOnlyCollection<IBattleDefinitionObject>> GetActiveBattleDefinitionsByPlayerAsync(Guid playerId)
        {
            var entities = await BattleDefinitionsRepository.GetActiveBattleDefinitionsByPlayer(playerId);
            var battleDefinitions = entities.Select(MutableBattleDefinitionObject.FromEntity).ToArray();
            await Task.WhenAll(battleDefinitions.Select(x => FillBattleDefinitionObject(x)));
            return battleDefinitions;
        }

        public async Task<IBattleDefinitionObject> GetBattleDefinitionByPlayerAndId(Guid playerId, Guid battleDefinitionId)
        {
            var battleDefinition =
                await BattleDefinitionsRepository.GetByPlayerAndId(playerId, battleDefinitionId);
            var battleDefinitionObject = MutableBattleDefinitionObject.FromEntity(battleDefinition);
            await FillBattleDefinitionObject(battleDefinitionObject);
            return battleDefinitionObject;
        }

        public async Task<IBattleDefinitionObject> CreateBattleDefinition(Guid playerId, int width, int height)
        {
            var container = await UnitsContainersService.Create(height, playerId);
            var entity = await BattleDefinitionsRepository.Create(playerId, width, height, container.Id);
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
                await PlayerUnitsService.GetAliveUnitsByContainerId(battleDefinitionObject.ContainerId);
            battleDefinitionObject.UnitsContainerObject = unitsContainerObject ??
                await UnitsContainersService.GetById(battleDefinitionObject.ContainerId);
        }
    }
}