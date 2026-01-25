using System;
using Epic.Data.Reward;
using JetBrains.Annotations;

namespace Epic.Data.RewardDefinitions
{
    public interface IRewardDefinitionEntity : IRewardDefinitionFields
    {
        Guid Id { get; }
    }

    public interface IRewardDefinitionFields
    {
        string Key { get; set; }
        int Value { get; }
        string Name { get; }
        string Description { get; }
        
        RewardType RewardType { get; }
        Guid[] Ids { get; }
        int[] MinAmounts { get; }
        int[] MaxAmounts { get; }
        string Message { get; }
        string IconUrl { get; }
        string Title { get; }
        
        bool CanDecline { get; }
        [CanBeNull] string GuardMessage { get; }
        Guid[] GuardUnitTypeIds { get; }
        int[] GuardUnitMinAmounts { get; }
        int[] GuardUnitMaxAmounts { get; }
        int GuardBattleMinWidth { get; }
        int GuardBattleMaxWidth { get; }
        int GuardBattleMinHeight { get; }
        int GuardBattleMaxHeight { get; }
    }

    public class RewardDefinitionFields : IRewardDefinitionFields
    {
        public string Key { get; set; }
        public int Value { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public RewardType RewardType { get; set; }
        public Guid[] Ids { get; set; }
        public int[] MinAmounts { get; set; }
        public int[] MaxAmounts { get; set; }
        public string Message { get; set; }
        public string IconUrl { get; set; }
        public string Title { get; set; }
        public bool CanDecline { get; set; }
        public string GuardMessage { get; set; }
        public Guid[] GuardUnitTypeIds { get; set; }
        public int[] GuardUnitMinAmounts { get; set; }
        public int[] GuardUnitMaxAmounts { get; set; }
        public int GuardBattleMinWidth { get; set; }
        public int GuardBattleMaxWidth { get; set; }
        public int GuardBattleMinHeight { get; set; }
        public int GuardBattleMaxHeight { get; set; }

        public void UpdateFrom(IRewardDefinitionFields fields)
        {
            Key = fields.Key;
            Value = fields.Value;
            Name = fields.Name;
            Description = fields.Description;
            RewardType = fields.RewardType;
            Ids = fields.Ids;
            MinAmounts = fields.MinAmounts;
            MaxAmounts = fields.MaxAmounts;
            Message = fields.Message;
            IconUrl = fields.IconUrl;
            Title = fields.Title;
            CanDecline = fields.CanDecline;
            GuardMessage = fields.GuardMessage;
            GuardUnitTypeIds = fields.GuardUnitTypeIds;
            GuardUnitMinAmounts = fields.GuardUnitMinAmounts;
            GuardUnitMaxAmounts = fields.GuardUnitMaxAmounts;
            GuardBattleMinWidth = fields.GuardBattleMinWidth;
            GuardBattleMaxWidth = fields.GuardBattleMaxWidth;
            GuardBattleMinHeight = fields.GuardBattleMinHeight;
            GuardBattleMaxHeight = fields.GuardBattleMaxHeight;
        }
    }

    public class RewardDefinitionEntity : RewardDefinitionFields, IRewardDefinitionEntity
    {
        public Guid Id { get; }

        public RewardDefinitionEntity(Guid id, IRewardDefinitionFields fields)
        {
            Id = id;
            UpdateFrom(fields);
        }
    }
}