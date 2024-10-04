using System;
using System.Threading.Tasks;
using Epic.Core.Objects.Battle;

namespace Epic.Core
{
    public interface IBattlesService
    {
        Task<IBattleObject> GetBattleById(Guid battleId);
        Task<IBattleObject> FindActiveBattleByUserId(Guid userId);

        Task<IBattleObject> CreateBattleFromDefinition(Guid userId, Guid battleDefinitionId);
        
        Task<IBattleObject> BeginBattle(Guid userId, IBattleObject battleObject);
    }
}