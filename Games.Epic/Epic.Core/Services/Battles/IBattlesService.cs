using System;
using System.Threading.Tasks;
using Epic.Core.Logic;

namespace Epic.Core.Services.Battles
{
    public interface IBattlesService
    {
        Task<IBattleObject> GetBattleById(Guid battleId);
        Task<IBattleObject> FindActiveBattleByPlayerId(Guid playerId);

        Task<IBattleObject> CreateBattleFromDefinition(Guid playerId, Guid battleDefinitionId);
        
        Task<IBattleObject> BeginBattle(Guid playerId, IBattleObject battleObject);
        Task UpdateBattle(IBattleObject battleObject);
        
        Task FinishBattle(IBattleObject battleObject, BattleResult result);
    }
}