using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.BattleObstacles;
using JetBrains.Annotations;

namespace Epic.Core.Services.BattleObstacles
{
    [UsedImplicitly]
    public class DefaultBattleObstaclesService : IBattleObstaclesService
    {
        public IBattleObstaclesRepository Repository { get; }

        public DefaultBattleObstaclesService([NotNull] IBattleObstaclesRepository repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        
        public async Task<IBattleObstacleObject[]> GetForBattle(Guid battleId)
        {
            var entities = await Repository.GetByBattleId(battleId);
            return entities
                .Select(MutableBattleObstacleObject.FromEntity)
                .ToArray<IBattleObstacleObject>();
        }

        public async Task<IBattleObstacleObject[]> CreateBatch(IEnumerable<IBattleObstacleFields> fields)
        {
            var entities = await Repository.CreateBatch(fields);
            return entities
                .Select(MutableBattleObstacleObject.FromEntity)
                .ToArray<IBattleObstacleObject>();
        }
    }
}