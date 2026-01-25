using System;
using System.Linq;
using Epic.Core.Services.ArtifactTypes;
using Epic.Data.Artifact;

namespace Epic.Core.Services.Artifacts
{
    public class MutableArtifactObject : IArtifactObject, IArtifactFields
    {
        public Guid Id { get; set; }
        public Guid TypeId { get; set; }
        public Guid HeroId { get; set; }
        public int[] EquippedSlotsIndexes { get; set; } = Array.Empty<int>();
        public IArtifactTypeObject ArtifactType { get; set; }

        private MutableArtifactObject() { }

        public static MutableArtifactObject FromEntity(IArtifactEntity entity)
        {
            return new MutableArtifactObject
            {
                Id = entity.Id,
                TypeId = entity.TypeId,
                HeroId = entity.HeroId,
                EquippedSlotsIndexes = entity.EquippedSlotsIndexes?.ToArray() ?? Array.Empty<int>(),
                ArtifactType = null,
            };
        }

        public IArtifactEntity ToEntity()
        {
            return ArtifactEntity.FromFields(Id, this);
        }
    }
}

