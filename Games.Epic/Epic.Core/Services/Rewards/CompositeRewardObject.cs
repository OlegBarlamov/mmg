using System;
using System.Collections.Generic;
using System.Linq;
using Epic.Core.Objects.Rewards;
using Epic.Core.Services.BattleDefinitions;
using Epic.Core.Services.UnitTypes;
using Epic.Data.GameResources;
using Epic.Data.Reward;
using JetBrains.Annotations;

namespace Epic.Core.Services.Rewards
{
    public class CompositeRewardObject : BaseRewardObject, IRewardObject
    {
        public CompositeRewardObject(IRewardEntity entity) : base(entity)
        {
        }
        
        public IReadOnlyList<IUnitTypeObject> UnitTypes { get; set; } = Array.Empty<IUnitTypeObject>();
        public IReadOnlyList<IGameResourceEntity> Resources { get; set; } = Array.Empty<IGameResourceEntity>();
        
        [CanBeNull] 
        public IBattleDefinitionObject NextBattleDefinition { get; set; } 

        public RewardDescription GetDescription()
        {
            var description = GetRawDescription();

            if (!string.IsNullOrWhiteSpace(CustomTitle))
            {
                description.Name = CustomTitle;
                description.Amount = string.Empty;
            }
            if (!string.IsNullOrWhiteSpace(CustomIconUrl))
                description.IconUrl = CustomIconUrl;

            return description;
        }
        
        private RewardDescription GetRawDescription()
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

            if (RewardType == RewardType.UnitToBuy)
            {
                return new RewardDescription
                {
                    Name = "Buy: " + string.Join(',', UnitTypes.Select(x => x.Name)),
                    Amount = Amounts.Sum().ToString(),
                    IconUrl = UnitTypes.FirstOrDefault()?.DashboardImgUrl ?? string.Empty,
                };
            }

            if (RewardType == RewardType.Battle)
            {
                return new RewardDescription
                {
                    Name = "Battle: " + string.Join(',', NextBattleDefinition!.Units.Select(x => x.UnitType.Name)),
                    Amount = NextBattleDefinition.Units.Sum(x => x.Count).ToString(),
                    IconUrl = NextBattleDefinition.Units.Select(x => x.UnitType).FirstOrDefault()?.DashboardImgUrl ??
                              string.Empty,
                };
            }
            
            throw new NotImplementedException();
        }
    }
}