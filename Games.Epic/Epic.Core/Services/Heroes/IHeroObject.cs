using System;
using System.Collections.Generic;
using System.Linq;
using Epic.Core.Services.Artifacts;
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
        
        
        bool HasAliveUnits { get; }
        IUnitsContainerObject ArmyContainer { get; }
        IReadOnlyList<IArtifactObject> Artifacts { get; }

        IReadOnlyList<IArtifactObject> GetEquippedArtefacts();
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
        public int Power { get; set; }
        public int Knowledge { get; set; }
        public int CurrentMana { get; set; }
        
        public bool HasAliveUnits { get; set; }
        public IUnitsContainerObject ArmyContainer { get; set; }
        public IReadOnlyList<IArtifactObject> Artifacts { get; set; }

        public IReadOnlyList<IArtifactObject> GetEquippedArtefacts()
        {
            return (Artifacts ?? Array.Empty<IArtifactObject>())
                .Where(a => a != null && a.EquippedSlotsIndexes != null && a.EquippedSlotsIndexes.Any(i => i >= 0))
                .ToArray();
        }

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
                Power = heroObject.Power,
                Knowledge = heroObject.Knowledge,
                CurrentMana = heroObject.CurrentMana,
                Artifacts = heroObject.Artifacts,
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
                Power = entity.Power,
                Knowledge = entity.Knowledge,
                CurrentMana = entity.CurrentMana,
                Artifacts = Array.Empty<IArtifactObject>(),
            };
        }
        
        public IHeroEntity ToEntity()
        {
            return MutableHeroEntity.FromFields(Id, this);
        }
    }
}