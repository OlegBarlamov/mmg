using System;
using Epic.Core.Objects;
using Epic.Core.Services.UnitsContainers;
using Epic.Data.Heroes;

namespace Epic.Core.Services.Heroes
{
    public interface IHeroObject : IGameObject<IHeroEntity>, IHeroStats
    {
        Guid Id { get; }
        string Name { get; }
        Guid ArmyContainerId { get; }
        bool IsKilled { get; }
        int Level { get; }
        int Experience { get; }
        
        
        bool HasAliveUnits { get; }
        IUnitsContainerObject ArmyContainer { get; }
    }

    public class MutableHeroObject : IHeroObject, IHeroEntityFields
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ArmyContainerId { get; set; }
        public bool IsKilled { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        
        
        
        public bool HasAliveUnits { get; set; }
        public IUnitsContainerObject ArmyContainer { get; set; }

        private MutableHeroObject() { }

        public static MutableHeroObject CopyFrom(IHeroObject heroObject)
        {
            return new MutableHeroObject
            {
                Id = heroObject.Id,
                Name = heroObject.Name,
                ArmyContainerId = heroObject.ArmyContainerId,
                IsKilled = heroObject.IsKilled,
                Level = heroObject.Level,
                Experience = heroObject.Experience,
                Attack = heroObject.Attack,
                Defense = heroObject.Defense,
            };
        }
        
        public static MutableHeroObject FromEntity(IHeroEntity entity)
        {
            return new MutableHeroObject
            {
                Id = entity.Id,
                Name = entity.Name,
                ArmyContainerId = entity.ArmyContainerId,
                IsKilled = entity.IsKilled,
                Level = entity.Level,
                Experience = entity.Experience,
                Attack = entity.Attack,
                Defense = entity.Defense,
            };
        }
        
        public IHeroEntity ToEntity()
        {
            return MutableHeroEntity.FromFields(Id, this);
        }
    }
}