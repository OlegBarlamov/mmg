using System;
using System.Threading.Tasks;

namespace Epic.Data.BattleDefinitions
{
    public interface IBattleDefinitionsRepository
    {
        Task<IBattleDefinitionEntity[]> GetBattleDefinitionsByUserAsync(Guid userId);
    }
}