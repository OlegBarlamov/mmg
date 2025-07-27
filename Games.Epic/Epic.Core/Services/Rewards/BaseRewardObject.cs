using System;
using Epic.Data.Reward;

namespace Epic.Core.Services.Rewards
{
    public abstract class BaseRewardObject : IRewardEntityFields
    {
        public Guid Id { get; protected set; }
        public Guid BattleDefinitionId { get; protected set; }
        public RewardType RewardType { get; protected set; }
        public Guid[] Ids { get; protected set; }
        public int[] Amounts { get; protected set; }
        public string Message { get; protected set; }
        public bool CanDecline { get; set; }
        public Guid? NextBattleDefinitionId { get; set; }
        public string CustomIconUrl { get; set; }
        public string CustomTitle { get; set; }

        protected BaseRewardObject(IRewardEntity entity)
        {
            Id = entity.Id;
            Amounts = entity.Amounts;
            BattleDefinitionId = entity.BattleDefinitionId;
            Message = entity.Message;
            RewardType = entity.RewardType;
            Ids = entity.Ids;
            CanDecline = entity.CanDecline;
            NextBattleDefinitionId = entity.NextBattleDefinitionId;
            CustomIconUrl = entity.CustomIconUrl;
            CustomTitle = entity.CustomTitle;
        }
        
        public IRewardEntity ToEntity()
        {
            throw new System.NotImplementedException();
        }
    }
}