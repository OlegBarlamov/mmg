using System;
using Epic.Data.Reward;

namespace Epic.Core.Objects.Rewards
{
    public interface IRewardObject
    {
        public Guid Id { get; }
        public Guid BattleDefinitionId { get; }
        public RewardType RewardType { get; }
        public Guid[] TypeIds { get; }
        public int[] Amounts { get; }
        public string Message { get; }
        
        RewardDescription GetDescription();
        
        IUnitTypeObject[] GetUnitTypes();
    }
}