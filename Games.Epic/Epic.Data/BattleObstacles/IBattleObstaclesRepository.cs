using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Data.BattleObstacles
{
    public interface IBattleObstaclesRepository : IRepository
    {
        Task<IBattleObstacleEntity[]> GetByBattleId(Guid battleId);
        
        Task<IBattleObstacleEntity[]> CreateBatch(IEnumerable<IBattleObstacleFields> fields);
    }
}