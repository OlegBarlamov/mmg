using System;
using System.Threading.Tasks;
using Epic.Core.Logic;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.BattleReports;
using Epic.Data.BattleReports;

namespace Epic.Core.Services.Battles
{
    public interface IBattlesService
    {
        Task<IBattleObject> GetBattleById(Guid battleId);
        Task<IBattleObject> FindActiveBattleByPlayerId(Guid playerId);

        Task<IBattleObject> CreateBattleFromDefinition(Guid playerId, Guid battleDefinitionId);
        Task<IBattleObject> CreateBattleFromDefinition(Guid playerId, IBattleDefinitionObject battleDefinitionObject, bool progressDays);
        
        Task<IBattleObject> BeginBattle(Guid playerId, IBattleObject battleObject);
        Task UpdateBattle(IBattleObject battleObject);
        
        Task<IBattleReportEntity> FinishBattle(IBattleObject battleObject, BattleResult result);
    }
}