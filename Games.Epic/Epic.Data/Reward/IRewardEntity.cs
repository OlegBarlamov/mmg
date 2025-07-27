using System;
using JetBrains.Annotations;

namespace Epic.Data.Reward
{
    public interface IRewardEntity : IRewardEntityFields
    {
        Guid Id { get; }
        Guid BattleDefinitionId { get; }
    }

    public interface IRewardEntityFields
    {
        RewardType RewardType { get; }
        Guid[] Ids { get; }
        int[] Amounts { get; }
        string Message { get; }
        bool CanDecline { get; }
        Guid? NextBattleDefinitionId { get; }
        [CanBeNull] string CustomIconUrl { get; }
        [CanBeNull] string CustomTitle { get; }
    }

    public class MutableRewardFields : IRewardEntityFields
    {
        public RewardType RewardType { get; set; }
        public Guid[] Ids { get; set; }
        public int[] Amounts { get; set; }
        public string Message { get; set; }
        public bool CanDecline { get; set; }
        public Guid? NextBattleDefinitionId { get; set; }
        public string CustomIconUrl { get; set; }
        public string CustomTitle { get; set; }

        public MutableRewardFields() { }
    }
    
    internal class MutableRewardEntity : MutableRewardFields, IRewardEntity
    {
        public Guid Id { get; }
        public Guid BattleDefinitionId { get; }

        internal MutableRewardEntity(Guid id, Guid battleDefinitionId)
        {
            Id = id;
            BattleDefinitionId = battleDefinitionId;
        }

        public void FillFromFields([NotNull] IRewardEntityFields fields)
        {
            RewardType = fields.RewardType;
            Ids = fields.Ids;
            Amounts = fields.Amounts;
            Message = fields.Message;
            CanDecline = fields.CanDecline;
            CustomIconUrl = fields.CustomIconUrl;
            CustomTitle = fields.CustomTitle;
            NextBattleDefinitionId = fields.NextBattleDefinitionId;
        }
    }
}