using System;
using System.Linq;
using Epic.Core.Objects.Rewards;
using Epic.Data.Reward;

namespace Epic.Server.Resources
{
    public class AcceptingRewardResource
    {
        public Guid Id { get; }
        public Guid BattleDefinitionId { get; }
        public RewardType RewardType { get; }
        public UnitRewardResource[] UnitRewardResources { get; }
        public string Message { get; }
        
        public AcceptingRewardResource(IRewardObject reward)
        {
            Id = reward.Id;
            BattleDefinitionId = reward.BattleDefinitionId;
            RewardType = reward.RewardType;
            Message = reward.Message;

            if (reward.RewardType == RewardType.UnitsGain)
            {
                UnitRewardResources = reward.GetUnitTypes()
                    .Select((x, i) => new UnitRewardResource(x, reward.Amounts[i]))
                    .ToArray();
            }
        }
    }
}