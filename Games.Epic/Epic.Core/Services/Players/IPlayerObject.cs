using System;
using Epic.Core.Objects;
using Epic.Core.Services.UnitsContainers;
using Epic.Data.Players;

namespace Epic.Core.Services.Players
{
    public interface IPlayerObject : IGameObject<IPlayerEntity>
    {
        Guid Id { get; }
        Guid UserId { get; }
        Guid ArmyContainerId { get; }
        Guid SupplyContainerId { get; }
        int Day { get; }
        string Name { get; }
        PlayerObjectType PlayerType { get; }
        bool IsDefeated { get; }
        bool GenerationInProgress { get; }
        IUnitsContainerObject Army { get; }
        IUnitsContainerObject Supply { get; }
    }

    internal class MutablePlayerObject : IPlayerObject
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ArmyContainerId { get; set; }
        public Guid SupplyContainerId { get; set; }
        public int Day { get; set; }
        public string Name { get; set; }
        public PlayerObjectType PlayerType { get; set; }
        public bool IsDefeated { get; set; }
        public bool GenerationInProgress { get; set; }
        
        public IUnitsContainerObject Army { get; set; }
        public IUnitsContainerObject Supply { get; set; }

        private MutablePlayerObject() {}

        public static MutablePlayerObject FromEntity(IPlayerEntity entity)
        {
            return new MutablePlayerObject
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Day = entity.Day,
                Name = entity.Name,
                PlayerType = entity.PlayerType.ToObjectType(),
                IsDefeated = entity.IsDefeated,
                GenerationInProgress = entity.GenerationInProgress,
                ArmyContainerId = entity.ArmyContainerId,
                SupplyContainerId = entity.SupplyContainerId,
            };
        }

        public static IPlayerEntity ToEntity(MutablePlayerObject mutablePlayerObject)
        {
            return new MutablePlayerEntity
            {
                Id = mutablePlayerObject.Id,
                UserId = mutablePlayerObject.UserId,
                Day = mutablePlayerObject.Day,
                Name = mutablePlayerObject.Name,
                PlayerType = mutablePlayerObject.PlayerType.ToEntity(),
                IsDefeated = mutablePlayerObject.IsDefeated,
                GenerationInProgress = mutablePlayerObject.GenerationInProgress,
                ArmyContainerId = mutablePlayerObject.ArmyContainerId,
                SupplyContainerId = mutablePlayerObject.SupplyContainerId,
            };
        }

        public IPlayerEntity ToEntity()
        {
            return ToEntity(this);
        }
    }
}
