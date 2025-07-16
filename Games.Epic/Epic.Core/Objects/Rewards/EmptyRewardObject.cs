using System;
using Epic.Data.Reward;

namespace Epic.Core.Objects.Rewards
{
    public class EmptyRewardObject : BaseRewardObject, IRewardObject
    {
        private EmptyRewardObject(IRewardEntity rewardEntity) : base(rewardEntity) { }
        
        internal static EmptyRewardObject FromEntity(IRewardEntity rewardEntity)
        {
            var reward = new EmptyRewardObject(rewardEntity);
            if (reward.RewardType != RewardType.None)
                throw new ArgumentException($"Reward object is not {RewardType.None} reward object.");
            
            reward.RewardType = RewardType.None;
            reward.TypeIds = Array.Empty<Guid>();
            reward.Amounts = Array.Empty<int>();
            
            return reward;
        }

        public RewardDescription GetDescription()
        {
            return new RewardDescription
            {
                Name = "",
                Amount = "",
                IconUrl = "",
            };
        }

        public IUnitTypeObject[] GetUnitTypes()
        {
            return Array.Empty<IUnitTypeObject>();
        }
    }
}