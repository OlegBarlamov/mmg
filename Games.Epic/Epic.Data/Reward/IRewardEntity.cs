using System;

namespace Epic.Data.Reward
{
    public interface IRewardEntity
    {
        Guid Id { get; }
        Guid BattleDefinitionId { get; }
        RewardType RewardType { get; }
        Guid[] Ids { get; }
        int[] Amounts { get; }
        string Message { get; }
    }
    
    public class MutableRewardEntity : IRewardEntity
    {
        public Guid Id { get; set; }
        public Guid BattleDefinitionId { get; set; }
        public RewardType RewardType { get; set; }
        public Guid[] Ids { get; set; }
        public int[] Amounts { get; set; }
        public string Message { get; set; }
    }
}