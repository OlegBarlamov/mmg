using System.Threading.Tasks;
using Epic.Core.Services.BattleObstacles;
using Epic.Core.Services.Battles;

namespace Epic.Core.Logic
{
    public interface IBattleObstaclesGenerator
    {
        Task<IBattleObstacleObject[]> GenerateForBattle(IBattleObject battleObject);
    }
}