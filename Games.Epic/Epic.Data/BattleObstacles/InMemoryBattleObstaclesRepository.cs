using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epic.Data.BattleObstacles
{
    public class InMemoryBattleObstaclesRepository : IBattleObstaclesRepository
    {
        public string Name => nameof(InMemoryBattleObstaclesRepository);
        public string EntityName => "BattleObstacles";
        
        private readonly List<BattleObstacleEntityEntity> _battleObstacles = new List<BattleObstacleEntityEntity>();
        
        public Task<IBattleObstacleEntity[]> GetByBattleId(Guid battleId)
        {
            return Task.FromResult(_battleObstacles
                .Where(x => x.BattleId == battleId)
                .ToArray<IBattleObstacleEntity>()
            );
        }

        public Task<IBattleObstacleEntity[]> CreateBatch(IEnumerable<IBattleObstacleFields> fields)
        {
            var entities = fields
                .Select(x => new BattleObstacleEntityEntity(Guid.NewGuid(), x))
                .ToArray();
            
            _battleObstacles.AddRange(entities);
            
            return Task.FromResult(entities.ToArray<IBattleObstacleEntity>());
        }
    }
}