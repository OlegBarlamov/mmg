using System;
using System.Collections.Generic;
using System.Linq;
using Epic.Core.Services.BuffTypes;
using Epic.Data.ArtifactType;
using Epic.Data.Artifacts;

namespace Epic.Core.Services.ArtifactTypes
{
    public class MutableArtifactTypeObject : IArtifactTypeObject, IArtifactTypeEntityFields
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public string ThumbnailUrl { get; set; }
        public ArtifactSlot[] Slots { get; set; } = Array.Empty<ArtifactSlot>();
        public Guid[] BuffTypeIds { get; set; } = Array.Empty<Guid>();
        public IReadOnlyList<IBuffTypeObject> BuffTypes { get; set; } = Array.Empty<IBuffTypeObject>();
        public int AttackBonus { get; set; }
        public int DefenseBonus { get; set; }

        private MutableArtifactTypeObject() { }

        public static MutableArtifactTypeObject FromEntity(IArtifactTypeEntity entity)
        {
            return new MutableArtifactTypeObject
            {
                Id = entity.Id,
                Key = entity.Key,
                Name = entity.Name,
                Value = entity.Value,
                ThumbnailUrl = entity.ThumbnailUrl,
                Slots = entity.Slots?.ToArray() ?? Array.Empty<ArtifactSlot>(),
                BuffTypeIds = entity.BuffTypeIds?.ToArray() ?? Array.Empty<Guid>(),
                BuffTypes = Array.Empty<IBuffTypeObject>(),
                AttackBonus = entity.AttackBonus,
                DefenseBonus = entity.DefenseBonus,
            };
        }

        public IArtifactTypeEntity ToEntity()
        {
            return ArtifactTypeEntity.FromFields(Id, this);
        }
    }
}

