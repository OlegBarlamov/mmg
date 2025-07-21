using System;
using System.Collections.Generic;
using System.Linq;
using Epic.Core.Services.Rewards;
using Epic.Core.Services.UnitTypes;
using Epic.Data.Reward;

namespace Epic.Core.Objects.Rewards
{
    public class UnitsGainRewardObject : BaseRewardObject, IRewardObject
    {
        public IReadOnlyList<IUnitTypeObject> Units { get; private set; }
        
        private UnitsGainRewardObject(IRewardEntity rewardEntity) : base(rewardEntity) { }

        internal static UnitsGainRewardObject FromEntity(IRewardEntity rewardEntity, IEnumerable<IUnitTypeObject> unitTypes)
        {
            var reward = new UnitsGainRewardObject(rewardEntity);
            if (reward.RewardType != RewardType.UnitsGain)
                throw new ArgumentException($"Reward object is not {RewardType.UnitsGain} reward object.");
            
            reward.Units = new List<IUnitTypeObject>(unitTypes);
            
            return reward;
        }

        public RewardDescription GetDescription()
        {
            return new RewardDescription
            {
                Name = string.Join(',', Units.Select(x => x.Name)),
                Amount = Amounts.Sum().ToString(),
                IconUrl = Units.FirstOrDefault()?.DashboardImgUrl ?? string.Empty,
            };
        }

        public IUnitTypeObject[] GetUnitTypes()
        {
            return Units.ToArray();
        }

        public IRewardEntity ToEntity()
        {
            throw new NotImplementedException();
        }
    }
}