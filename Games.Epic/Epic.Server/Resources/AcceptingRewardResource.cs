using System;
using System.Collections.Generic;
using System.Linq;
using Epic.Core.Services.Rewards;
using Epic.Data.GameResources;
using Epic.Logic.Descriptions;

namespace Epic.Server.Resources
{
    public class AcceptingRewardResource
    {
        public Guid Id { get; }
        public Guid BattleDefinitionId { get; }
        public string RewardType { get; }
        public int[] Amounts { get; }
        public string Message { get; }
        public string IconUrl { get; }
        public string Title { get; }
        public UnitRewardResource[] UnitsRewards { get; }
        public ResourceDashboardResource[] ResourcesRewards { get; }
        public PriceResource[] Prices { get; }
        public BattleDefinitionResource NextBattle { get; }
        public bool CanDecline { get; }

        public AcceptingRewardResource(IRewardObject reward, IReadOnlyList<ResourceAmount[]> prices, Guid goldResourceId)
        {
            Id = reward.Id;
            BattleDefinitionId = reward.BattleDefinitionId;
            RewardType = reward.RewardType.ToString();
            Message = reward.Message;
            Amounts = reward.Amounts;
            CanDecline = reward.CanDecline;
            IconUrl = reward.CustomIconUrl;
            Title = reward.CustomTitle;

            UnitsRewards = reward.UnitTypes
                .Select((x, i) => new UnitRewardResource(x, reward.Amounts[i]))
                .ToArray();
            
            ResourcesRewards = reward.Resources
                .Select((x, i) => new ResourceDashboardResource(ResourceAmount.Create(x, reward.Amounts[i])))
                .ToArray();

            if (reward.NextBattleDefinition != null)
                NextBattle = new BattleDefinitionResource(
                    reward.NextBattleDefinition,
                    Array.Empty<IRewardObject>(), 
                    DescriptionVisibility.Full,
                    DescriptionVisibility.Full,
                    goldResourceId);

            Prices = prices.Select(x => new PriceResource(x)).ToArray();
        }
    }
}