using System;
using Epic.Core.Objects;
using Epic.Core.Services.Heroes;
using Epic.Core.Services.UnitsContainers;
using Epic.Data.Players;
using JetBrains.Annotations;

namespace Epic.Core.Services.Players
{
    public interface IPlayerObject : IGameObject<IPlayerEntity>
    {
        Guid Id { get; }
        Guid UserId { get; }
        Guid SupplyContainerId { get; }
        Guid? ActiveHeroId { get; }
        int Day { get; }
        int Stage { get; }
        string Name { get; }
        PlayerObjectType PlayerType { get; }
        bool GenerationInProgress { get; }
        int[] StageUnlockedDays { get; }
        
        
        IUnitsContainerObject Supply { get; }
        IHeroObject ActiveHero { get; }
    }

    internal class MutablePlayerObject : IPlayerObject
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid SupplyContainerId { get; set; }
        public Guid? ActiveHeroId { get; set; }
        public int Day { get; set; }
        public int Stage { get; set; }
        public string Name { get; set; }
        public PlayerObjectType PlayerType { get; set; }
        public bool GenerationInProgress { get; set; }
        public int[] StageUnlockedDays { get; set; } = new[] { 1 };
        
        
        public IUnitsContainerObject Supply { get; set; }
        [CanBeNull] public IHeroObject ActiveHero { get; set; }

        private MutablePlayerObject() {}

        public static MutablePlayerObject CopyFrom(IPlayerObject playerObject)
        {
            return new MutablePlayerObject
            {
                Id = playerObject.Id,
                UserId = playerObject.UserId,
                Day = playerObject.Day,
                Stage = playerObject.Stage,
                Name = playerObject.Name,
                PlayerType = playerObject.PlayerType,
                GenerationInProgress = playerObject.GenerationInProgress,
                SupplyContainerId = playerObject.SupplyContainerId,
                ActiveHeroId = playerObject.ActiveHeroId,
                StageUnlockedDays = playerObject.StageUnlockedDays,
                
                Supply = playerObject.Supply,
                ActiveHero = playerObject.ActiveHero,
            };
        }
        
        public static MutablePlayerObject FromEntity(IPlayerEntity entity)
        {
            return new MutablePlayerObject
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Day = entity.Day,
                Stage = entity.Stage,
                Name = entity.Name,
                PlayerType = entity.PlayerType.ToObjectType(),
                GenerationInProgress = entity.GenerationInProgress,
                SupplyContainerId = entity.SupplyContainerId,
                ActiveHeroId = entity.ActiveHeroId,
                StageUnlockedDays = entity.StageUnlockedDays,
            };
        }

        public static IPlayerEntity ToEntity(MutablePlayerObject mutablePlayerObject)
        {
            return new MutablePlayerEntity(mutablePlayerObject.Id)
            {
                UserId = mutablePlayerObject.UserId,
                Day = mutablePlayerObject.Day,
                Stage = mutablePlayerObject.Stage,
                Name = mutablePlayerObject.Name,
                PlayerType = mutablePlayerObject.PlayerType.ToEntity(),
                GenerationInProgress = mutablePlayerObject.GenerationInProgress,
                SupplyContainerId = mutablePlayerObject.SupplyContainerId,
                ActiveHeroId = mutablePlayerObject.ActiveHeroId,
                StageUnlockedDays = mutablePlayerObject.StageUnlockedDays,
            };
        }

        public IPlayerEntity ToEntity()
        {
            return ToEntity(this);
        }
    }
}
