using System;
using System.Threading.Tasks;
using Epic.Data.Reward;
using Epic.Data.RewardDefinitions;

namespace Epic.Core.Services.RewardDefinitions
{
    public interface IRewardDefinitionsService
    {
        Task<IRewardEntity[]> CreateRewardsFromDefinition(IRewardDefinitionEntity rewardDefinitionEntity, Guid battleDefinitionId, int rewardFactor); 
    }
}