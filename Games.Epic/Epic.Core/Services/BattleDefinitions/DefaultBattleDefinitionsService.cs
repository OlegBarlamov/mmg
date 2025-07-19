using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.Units;
using Epic.Data.BattleDefinitions;
using JetBrains.Annotations;

namespace Epic.Core.Services.BattleDefinitions
{
    [UsedImplicitly]
    public class DefaultBattleDefinitionsService : IBattleDefinitionsService
    {
        public IBattleDefinitionsRepository BattleDefinitionsRepository { get; }
        public IPlayerUnitsService PlayerUnitsService { get; }

        public DefaultBattleDefinitionsService([NotNull] IBattleDefinitionsRepository battleDefinitionsRepository,
            [NotNull] IPlayerUnitsService playerUnitsService)
        {
            BattleDefinitionsRepository = battleDefinitionsRepository ?? throw new ArgumentNullException(nameof(battleDefinitionsRepository));
            PlayerUnitsService = playerUnitsService ?? throw new ArgumentNullException(nameof(playerUnitsService));
        }
        
        public async Task<IReadOnlyCollection<IBattleDefinitionObject>> GetActiveBattleDefinitionsByPlayerAsync(Guid playerId)
        {
            var entities = await BattleDefinitionsRepository.GetActiveBattleDefinitionsByPlayer(playerId);
            var battleDefinitions = entities.Select(MutableBattleDefinitionObject.FromEntity).ToArray();
            await Task.WhenAll(battleDefinitions.Select(FillBattleDefinitionObject));
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

        public Task SetFinished(Guid battleDefinitionId)
        {
            return BattleDefinitionsRepository.SetFinished(battleDefinitionId);
        }

        private Task FillBattleDefinitionObject(MutableBattleDefinitionObject battleDefinitionObject)
        {
            return PlayerUnitsService
                .GetUnitsByIds(battleDefinitionObject.UnitsIds)
                .ContinueWith(task => battleDefinitionObject.Units = task.Result);
        }
    }
}