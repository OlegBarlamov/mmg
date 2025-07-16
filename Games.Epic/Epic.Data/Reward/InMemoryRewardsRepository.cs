using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Epic.Data.Reward
{
    [UsedImplicitly]
    public class InMemoryRewardsRepository : IRewardsRepository
    {
        public string Name => "Reward";
        public string EntityName => nameof(InMemoryRewardsRepository);
        
        private readonly List<MutableRewardEntity> _rewards = new List<MutableRewardEntity>();
        private readonly List<UserRewardEntity> _userRewards = new List<UserRewardEntity>();
        
        public Task<IRewardEntity[]> GetRewardsByIdAsync(Guid[] ids)
        {
            return Task.FromResult(_rewards.Where(x => ids.Contains(x.Id)).ToArray<IRewardEntity>());
        }

        public Task<IRewardEntity[]> GetRewardsByBattleDefinitionId(Guid battleDefinitionId)
        {
            return Task.FromResult(_rewards.Where(x => x.BattleDefinitionId == battleDefinitionId).ToArray<IRewardEntity>());
        }

        public Task<IRewardEntity[]> FindNotAcceptedRewardsByUserIdAsync(Guid userId)
        {
            var rewardIds = _userRewards.Where(x => x.UserId == userId && !x.Accepted).Select(x => x.RewardId);
            var rewards = _rewards.Where(x => rewardIds.Contains(x.Id)).ToArray<IRewardEntity>();
            return Task.FromResult(rewards);
        }

        public Task<IRewardEntity> RemoveRewardFromUser(Guid userId, Guid rewardId)
        {
            var reward = _userRewards.First(x => x.UserId == userId && x.RewardId == rewardId);
            _userRewards.Remove(reward);
            
            return Task.FromResult<IRewardEntity>(_rewards.First(x => x.Id == rewardId));
        }

        public Task<IRewardEntity> CreateRewardAsync(Guid battleDefinitionId, RewardType rewardType, Guid[] typeIds, int[] amounts, string message)
        {
            var newReward = new MutableRewardEntity
            {
                Id = Guid.NewGuid(),
                BattleDefinitionId = battleDefinitionId,
                RewardType = rewardType,
                TypeIds = typeIds,
                Amounts = amounts,
                Message = message,
            };
            _rewards.Add(newReward);
            
            return Task.FromResult<IRewardEntity>(newReward);
        }

        public Task GiveRewardsToUserAsync(Guid[] rewardIds, Guid userId)
        {
            var records = rewardIds.Select(id => new UserRewardEntity
            {
                Id = Guid.NewGuid(),
                RewardId = id,
                UserId = userId,
                Accepted = false,
            }).ToArray();
            _userRewards.AddRange(records);

            return Task.CompletedTask;
        }

        public Task SetRewardAsAccepted(Guid rewardId, Guid userId, bool accepted)
        {
            var targetReward = _userRewards.First(x => x.Id == rewardId && x.UserId == userId);
            targetReward.Accepted = accepted;
            return Task.CompletedTask;
        }
    }
}