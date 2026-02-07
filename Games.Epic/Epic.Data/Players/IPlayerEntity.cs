using System;

namespace Epic.Data.Players
{
    public interface IPlayerEntity : IPlayerEntityFields
    {
        Guid Id { get; }
    }

    public interface IPlayerEntityFields
    {
        Guid UserId { get; }
        Guid SupplyContainerId { get; }
        Guid? ActiveHeroId { get; }
        int Day { get; }
        int Stage { get; }
        string Name { get; }
        PlayerEntityType PlayerType { get; }
        bool GenerationInProgress { get; }
        /// <summary>
        /// Array storing the day when each stage was unlocked.
        /// Index corresponds to stage number. Stage 0 is unlocked at day 1 by default.
        /// Used for calculating effective day for difficulty.
        /// </summary>
        int[] StageUnlockedDays { get; }
    }

    public class MutablePlayerEntityFields : IPlayerEntityFields
    {
        public Guid UserId { get; set; }
        public Guid ArmyContainerId { get; set; }
        public Guid SupplyContainerId { get; set; }
        public Guid? ActiveHeroId { get; set; }
        public int Day { get; set; }
        public int Stage { get; set; }
        public string Name { get; set; }
        public PlayerEntityType PlayerType { get; set; }
        public bool GenerationInProgress { get; set; }
        public int[] StageUnlockedDays { get; set; } = new[] { 1 };

        public MutablePlayerEntityFields() { }
    }
    
    public class MutablePlayerEntity : MutablePlayerEntityFields, IPlayerEntity
    {
        public Guid Id { get; }

        public MutablePlayerEntity(Guid id)
        {
            Id = id;
        }

        public void UpdateFrom(IPlayerEntityFields entity)
        {
            Name = entity.Name;
            UserId = entity.UserId;
            PlayerType = entity.PlayerType;
            Day = entity.Day;
            Stage = entity.Stage;
            ActiveHeroId = entity.ActiveHeroId;
            GenerationInProgress = entity.GenerationInProgress;
            SupplyContainerId = entity.SupplyContainerId;
            StageUnlockedDays = entity.StageUnlockedDays;
        }

        public static MutablePlayerEntity FromFields(Guid id, IPlayerEntityFields fields)
        {
            return new MutablePlayerEntity(id)
            {
                Name = fields.Name,
                UserId = fields.UserId,
                PlayerType = fields.PlayerType,
                Day = fields.Day,
                Stage = fields.Stage,
                ActiveHeroId = fields.ActiveHeroId,
                GenerationInProgress = fields.GenerationInProgress,
                SupplyContainerId = fields.SupplyContainerId,
                StageUnlockedDays = fields.StageUnlockedDays,
            };
        }
    }
}