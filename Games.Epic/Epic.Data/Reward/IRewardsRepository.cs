using System;
using System.Threading.Tasks;

namespace Epic.Data.Reward
{
    public interface IRewardsRepository : IRepository
    {
        Task<IRewardEntity[]> GetRewardsByIdAsync(Guid[] ids);
        Task<IRewardEntity[]> GetRewardsByBattleDefinitionId(Guid battleDefinitionId);
        Task<IRewardEntity[]> FindNotAcceptedRewardsByUserIdAsync(Guid userId);
        Task<IRewardEntity> RemoveRewardFromUser(Guid userId, Guid rewardId);
        Task<IRewardEntity> CreateRewardAsync(Guid battleDefinitionId, RewardType rewardType, Guid[] typeIds, int[] amounts, string message);
        Task GiveRewardsToUserAsync(Guid[] rewardIds, Guid userId);
        Task SetRewardAsAccepted(Guid rewardId, Guid userId, bool accepted);
    }
}