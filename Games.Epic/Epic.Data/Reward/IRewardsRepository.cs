using System;
using System.Threading.Tasks;

namespace Epic.Data.Reward
{
    public interface IRewardsRepository : IRepository
    {
        Task<IRewardEntity[]> GetRewardsByIdAsync(Guid[] ids);
        Task<IRewardEntity[]> GetRewardsByBattleDefinitionId(Guid battleDefinitionId);
        Task<IRewardEntity[]> FindNotAcceptedRewardsByPlayerId(Guid playerId);
        Task<IRewardEntity> RemoveRewardFromPlayer(Guid playerId, Guid rewardId);
        Task<IRewardEntity> CreateRewardAsync(Guid battleDefinitionId, RewardType rewardType, Guid[] typeIds, int[] amounts, string message);
        Task GiveRewardsToPlayerAsync(Guid[] rewardIds, Guid playerId);
        Task SetRewardAsAccepted(Guid rewardId, Guid playerId, bool accepted);
    }
}