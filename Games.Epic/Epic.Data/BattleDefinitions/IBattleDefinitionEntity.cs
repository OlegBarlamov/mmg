using System;

namespace Epic.Data.BattleDefinitions
{
    public interface IBattleDefinitionFields
    {
        int Width { get; }
        int Height { get; }
        
        Guid ContainerId { get; }
        public bool Finished { get; }
        DateTime CreatedAt { get; }
        int ExpireAtDay { get; }
        DateTime? ExpireAt { get; }
        int RewardVisibility { get; }
        int GuardVisibility { get; }
    }
    
    public interface IBattleDefinitionEntity : IBattleDefinitionFields
    {
        Guid Id { get; }
    }

    public class BattleDefinitionEntityFields : IBattleDefinitionFields
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Guid ContainerId { get; set; }
        public bool Finished { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ExpireAtDay { get; set; }
        public DateTime? ExpireAt { get; set; }
        public int RewardVisibility { get; set; }
        public int GuardVisibility { get; set; }

        public BattleDefinitionEntityFields()
        {
        }
    }

    internal class BattleDefinitionEntity : BattleDefinitionEntityFields, IBattleDefinitionEntity
    {
        public Guid Id { get; }

        private BattleDefinitionEntity(Guid id)
        {
            Id = id;
        }

        public static BattleDefinitionEntity FromFields(IBattleDefinitionFields fields)
        {
            return new BattleDefinitionEntity(Guid.NewGuid())
            {
                Width = fields.Width,
                Height = fields.Height,
                ContainerId = fields.ContainerId,
                Finished = fields.Finished,
                CreatedAt = fields.CreatedAt,
                ExpireAtDay = fields.ExpireAtDay,
                ExpireAt = fields.ExpireAt,
                RewardVisibility = fields.RewardVisibility,
                GuardVisibility = fields.GuardVisibility
            };
        }
    }
}