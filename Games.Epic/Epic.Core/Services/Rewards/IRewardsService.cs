using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epic.Core.Objects.Rewards;

namespace Epic.Core.Services.Rewards
{
    public interface IRewardsService
    {
        Task<IRewardObject[]> GetNotAcceptedUserRewards(Guid userId);
        Task<IReadOnlyDictionary<Guid, IRewardObject[]>> GetRewardsFromBattleDefinitions(Guid[] battleDefinitionIds);
        Task<IRewardObject[]> GetRewardsFromBattleDefinition(Guid battleDefinitionId);
        Task GiveRewardsToUserAsync(Guid[] rewardIds, Guid userId);
        Task<AcceptedRewardData> AcceptRewardAsync(Guid rewardId, Guid userId, int[] amounts);
        Task<AcceptedRewardData> RejectRewardAsync(Guid rewardId, Guid userId);
    }
}