using System;
using System.Linq;
using Epic.Core.Services.Artifacts;
using Epic.Data.Artifacts;

namespace Epic.Server.Resources
{
    public class ArtifactResource
    {
        public Guid Id { get; }
        public Guid TypeId { get; }
        public string TypeKey { get; }
        public string TypeName { get; }
        public string ThumbnailUrl { get; }
        public ArtifactSlot[] Slots { get; }
        public int AttackBonus { get; }
        public int DefenseBonus { get; }
        public int[] EquippedSlotsIndexes { get; }

        public ArtifactResource(IArtifactObject artifact)
        {
            Id = artifact.Id;
            TypeId = artifact.TypeId;
            TypeKey = artifact.ArtifactType?.Key;
            TypeName = artifact.ArtifactType?.Name;
            ThumbnailUrl = artifact.ArtifactType?.ThumbnailUrl;
            Slots = artifact.ArtifactType?.Slots?.ToArray() ?? Array.Empty<ArtifactSlot>();
            AttackBonus = artifact.ArtifactType?.AttackBonus ?? 0;
            DefenseBonus = artifact.ArtifactType?.DefenseBonus ?? 0;
            EquippedSlotsIndexes = artifact.EquippedSlotsIndexes?.ToArray() ?? Array.Empty<int>();
        }
    }
}

