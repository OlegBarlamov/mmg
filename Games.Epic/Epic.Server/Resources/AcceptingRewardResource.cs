using System;
using System.Collections.Generic;
using System.Linq;
using Epic.Core.Objects.Rewards;
using Epic.Core.Services.Rewards;
using Epic.Data.GameResources;
using Epic.Data.Reward;

namespace Epic.Server.Resources
{
    public class AcceptingRewardResource
    {
        public Guid Id { get; }
        public Guid BattleDefinitionId { get; }
        public string RewardType { get; }
        public int[] Amounts { get; }
        public string Message { get; }
        public UnitRewardResource[] UnitsRewards { get; }
        public ResourceDashboardResource[] ResourcesRewards { get; }
        public PriceResource[] Prices { get; }

        public AcceptingRewardResource(IRewardObject reward, IReadOnlyList<ResourceAmount[]> prices)
        {
            Id = reward.Id;
            BattleDefinitionId = reward.BattleDefinitionId;
            RewardType = reward.RewardType.ToString();
            Message = reward.Message;
            Amounts = reward.Amounts;

            UnitsRewards = reward.UnitTypes
                .Select((x, i) => new UnitRewardResource(x, reward.Amounts[i]))
                .ToArray();

            ResourcesRewards = reward.Resources
                .Select((x, i) => new ResourceDashboardResource(ResourceAmount.Create(x, reward.Amounts[i])))
                .ToArray();

            Prices = prices.Select(x => new PriceResource(x)).ToArray();
        }
    }
}