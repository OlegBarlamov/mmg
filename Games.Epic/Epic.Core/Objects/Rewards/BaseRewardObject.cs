using System;
using Epic.Data.Reward;

namespace Epic.Core.Objects.Rewards
{
    public abstract class BaseRewardObject
    {
        public Guid Id { get; protected set; }
        public Guid BattleDefinitionId { get; protected set; }
        public RewardType RewardType { get; protected set; }
        public Guid[] TypeIds { get; protected set; }
        public int[] Amounts { get; protected set; }
        public string Message { get; protected set; }
        
        protected BaseRewardObject(IRewardEntity entity)
        {
            Id = entity.Id;
            Amounts = entity.Amounts;
            BattleDefinitionId = entity.BattleDefinitionId;
            Message = entity.Message;
            RewardType = entity.RewardType;
            TypeIds = entity.TypeIds;
        }
    }
}