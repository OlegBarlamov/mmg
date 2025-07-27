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
        string Name { get; }
        PlayerEntityType PlayerType { get; }
        bool GenerationInProgress { get; }
    }

    public class MutablePlayerEntityFields : IPlayerEntityFields
    {
        public Guid UserId { get; set; }
        public Guid ArmyContainerId { get; set; }
        public Guid SupplyContainerId { get; set; }
        public Guid? ActiveHeroId { get; set; }
        public int Day { get; set; }
        public string Name { get; set; }
        public PlayerEntityType PlayerType { get; set; }
        public bool GenerationInProgress { get; set; }

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
            ActiveHeroId = entity.ActiveHeroId;
            GenerationInProgress = entity.GenerationInProgress;
            SupplyContainerId = entity.SupplyContainerId;
        }

        public static MutablePlayerEntity FromFields(Guid id, IPlayerEntityFields fields)
        {
            return new MutablePlayerEntity(id)
            {
                Name = fields.Name,
                UserId = fields.UserId,
                PlayerType = fields.PlayerType,
                Day = fields.Day,
                ActiveHeroId = fields.ActiveHeroId,
                GenerationInProgress = fields.GenerationInProgress,
                SupplyContainerId = fields.SupplyContainerId,
            };
        }
    }
}