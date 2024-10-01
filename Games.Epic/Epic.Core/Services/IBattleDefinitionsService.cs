using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epic.Core
{
    public interface IBattleDefinitionsService
    {
        Task<IReadOnlyCollection<IBattleDefinitionObject>> GetBattleDefinitionsByUserAsync(Guid userId);
    }
}