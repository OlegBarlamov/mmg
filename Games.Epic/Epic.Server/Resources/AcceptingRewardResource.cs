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
        public string GuardMessage { get; }
        public BattleDefinitionResource GuardBattle { get; }
        public bool CanDecline { get; }

        public AcceptingRewardResource(IRewardObject reward, IReadOnlyList<ResourceAmount[]> prices, Guid goldResourceId)
        {
            Id = reward.Id;
            BattleDefinitionId = reward.BattleDefinitionId;
            RewardType = reward.RewardType.ToString();
            Message = reward.Message;
            Amounts = reward.Amounts;
            CanDecline = reward.CanDecline;
            IconUrl = reward.IconUrl;
            Title = reward.Title;

            UnitsRewards = reward.UnitTypes
                .Select((x, i) => new UnitRewardResource(x, reward.Amounts[i]))
                .ToArray();
            
            ResourcesRewards = reward.Resources
                .Select((x, i) => new ResourceDashboardResource(ResourceAmount.Create(x, reward.Amounts[i])))
                .ToArray();

            if (reward.GuardBattleDefinition != null)
            {
                GuardMessage = reward.GuardMessage;
                GuardBattle = new BattleDefinitionResource(
                    reward.GuardBattleDefinition,
                    Array.Empty<IRewardObject>(),
                    DescriptionVisibility.MaskedSize,
                    DescriptionVisibility.MaskedSize,
                    goldResourceId);
            }

            Prices = prices.Select(x => new PriceResource(x)).ToArray();
        }
    }
}