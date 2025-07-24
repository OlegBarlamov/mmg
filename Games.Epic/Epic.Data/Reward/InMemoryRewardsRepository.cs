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
        private readonly List<PlayerRewardEntity> _playerRewards = new List<PlayerRewardEntity>();
        
        public Task<IRewardEntity[]> GetRewardsByIdAsync(IReadOnlyList<Guid> ids)
        {
            return Task.FromResult(_rewards.Where(x => ids.Contains(x.Id)).ToArray<IRewardEntity>());
        }

        public Task<IRewardEntity[]> GetRewardsByBattleDefinitionId(Guid battleDefinitionId)
        {
            return Task.FromResult(_rewards.Where(x => x.BattleDefinitionId == battleDefinitionId).ToArray<IRewardEntity>());
        }

        public Task<IRewardEntity[]> FindNotAcceptedRewardsByPlayerId(Guid playerId)
        {
            var rewardIds = _playerRewards.Where(x => x.PlayerId == playerId && !x.Accepted).Select(x => x.RewardId);
            var rewards = _rewards.Where(x => rewardIds.Contains(x.Id)).ToArray<IRewardEntity>();
            return Task.FromResult(rewards);
        }

        public Task<IRewardEntity> RemoveRewardFromPlayer(Guid playerId, Guid rewardId)
        {
            var reward = _playerRewards.First(x => x.PlayerId == playerId && x.RewardId == rewardId);
            _playerRewards.Remove(reward);
            
            return Task.FromResult<IRewardEntity>(_rewards.First(x => x.Id == rewardId));
        }

        public Task<IRewardEntity> CreateRewardAsync(Guid battleDefinitionId, RewardType rewardType, IReadOnlyList<Guid> typeIds, int[] amounts, string message)
        {
            var newReward = new MutableRewardEntity
            {
                Id = Guid.NewGuid(),
                BattleDefinitionId = battleDefinitionId,
                RewardType = rewardType,
                Ids = typeIds.ToArray(),
                Amounts = amounts,
                Message = message,
            };
            _rewards.Add(newReward);
            
            return Task.FromResult<IRewardEntity>(newReward);
        }

        public Task GiveRewardsToPlayerAsync(IReadOnlyList<Guid> rewardIds, Guid playerId)
        {
            var records = rewardIds.Select(id => new PlayerRewardEntity
            {
                Id = Guid.NewGuid(),
                RewardId = id,
                PlayerId = playerId,
                Accepted = false,
            }).ToArray();
            _playerRewards.AddRange(records);

            return Task.CompletedTask;
        }

        public Task SetRewardAsAccepted(Guid rewardId, Guid playerId, bool accepted)
        {
            var targetReward = _playerRewards.First(x => x.Id == rewardId && x.PlayerId == playerId);
            targetReward.Accepted = accepted;
            return Task.CompletedTask;
        }
    }
}