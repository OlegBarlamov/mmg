using System.Collections.Generic;
using System.Linq;
using Epic.Data.RewardDefinitions;

namespace Epic.Core.Services.RewardDefinitions
{
    public interface IRewardDefinitionsRegistry
    {
        IReadOnlyList<IGrouping<string, IRewardDefinitionEntity>> AllOrdered { get; }
    }
}