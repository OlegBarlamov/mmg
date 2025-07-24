using System;
using System.Collections.Generic;
using System.Linq;
using Epic.Core.Objects.Rewards;
using Epic.Core.Services.UnitTypes;
using Epic.Data.GameResources;
using Epic.Data.Reward;

namespace Epic.Core.Services.Rewards
{
    public class CompositeRewardObject : BaseRewardObject, IRewardObject
    {
        public CompositeRewardObject(IRewardEntity entity) : base(entity)
        {
        }
        
        public IReadOnlyList<IUnitTypeObject> UnitTypes { get; set; } = Array.Empty<IUnitTypeObject>();
        public IReadOnlyList<IGameResourceEntity> Resources { get; set; } = Array.Empty<IGameResourceEntity>();

        public RewardDescription GetDescription()
        {
            if (RewardType == RewardType.None)
            {
                return new RewardDescription();
            }
            
            if (RewardType == RewardType.UnitsGain)
            {
                return new RewardDescription
                {
                    Name = string.Join(',', UnitTypes.Select(x => x.Name)),
                    Amount = Amounts.Sum().ToString(),
                    IconUrl = UnitTypes.FirstOrDefault()?.DashboardImgUrl ?? string.Empty,
                };
            }

            if (RewardType == RewardType.ResourcesGain)
            {
                return new RewardDescription
                {
                    Name = string.Join(',', Resources.Select(x => x.Name)),
                    Amount = string.Join(',', Amounts.Select(x => x.ToString())),
                    IconUrl = Resources.FirstOrDefault()?.IconUrl ?? string.Empty,
                };
            }

            throw new NotImplementedException();
        }
    }
}