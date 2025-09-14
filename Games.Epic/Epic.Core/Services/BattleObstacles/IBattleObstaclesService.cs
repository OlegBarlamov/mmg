using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epic.Data.BattleObstacles;

namespace Epic.Core.Services.BattleObstacles
{
    public interface IBattleObstaclesService
    {
        Task<IBattleObstacleObject[]> GetForBattle(Guid battleId);
        
        Task<IBattleObstacleObject[]> CreateBatch(IEnumerable<IBattleObstacleFields> fields);
    }
}