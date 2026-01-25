using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epic.Core.Objects.Rewards;
using Epic.Core.Services.Battles;

namespace Epic.Core.Services.Rewards
{
    public interface IRewardsService
    {
        Task<IRewardObject[]> GetNotAcceptedPlayerRewards(Guid playerId);
        Task<IReadOnlyDictionary<Guid, IRewardObject[]>> GetRewardsFromBattleDefinitions(Guid[] battleDefinitionIds);
        Task<IRewardObject[]> GetRewardsFromBattleDefinition(Guid battleDefinitionId);
        Task GiveRewardsToPlayerAsync(Guid[] rewardIds, Guid playerId);
        Task<AcceptedRewardData> AcceptRewardAsync(Guid rewardId, Guid playerId, int[] amounts, int[] affectedSlots);
        Task<AcceptedRewardData> RejectRewardAsync(Guid rewardId, Guid playerId);
        Task<IBattleObject> BeginRewardGuardBattle(Guid rewardId, Guid playerId);
        Task RejectRewardsGuardedByBattleDefinition(Guid playerId, Guid guardBattleDefinitionId);
    }
}