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
        public string GuardMessage { get; set; }
        public Guid? GuardBattleDefinitionId { get; set; }
        public string IconUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        protected BaseRewardObject(IRewardEntity entity)
        {
            Id = entity.Id;
            Amounts = entity.Amounts;
            BattleDefinitionId = entity.BattleDefinitionId;
            Message = entity.Message;
            RewardType = entity.RewardType;
            Ids = entity.Ids;
            CanDecline = entity.CanDecline;
            GuardBattleDefinitionId = entity.GuardBattleDefinitionId;
            IconUrl = entity.IconUrl;
            Title = entity.Title;
            Description = entity.Description;
            GuardMessage = entity.GuardMessage;
        }
        
        public IRewardEntity ToEntity()
        {
            throw new System.NotImplementedException();
        }
    }
}