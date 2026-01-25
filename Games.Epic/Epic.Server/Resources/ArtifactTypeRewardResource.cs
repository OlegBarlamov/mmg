using System;
using System.Linq;
using Epic.Core.Services.ArtifactTypes;
using Epic.Data.Artifacts;

namespace Epic.Server.Resources
{
    public class ArtifactTypeRewardResource
    {
        public Guid Id { get; }
        public string Key { get; }
        public string Name { get; }
        public string ThumbnailUrl { get; }
        public ArtifactSlot[] Slots { get; }
        public int AttackBonus { get; }
        public int DefenseBonus { get; }
        public int Amount { get; }

        public ArtifactTypeRewardResource(IArtifactTypeObject artifactType, int amount)
        {
            Id = artifactType.Id;
            Key = artifactType.Key;
            Name = artifactType.Name;
            ThumbnailUrl = artifactType.ThumbnailUrl;
            Slots = artifactType.Slots?.ToArray() ?? Array.Empty<ArtifactSlot>();
            AttackBonus = artifactType.AttackBonus;
            DefenseBonus = artifactType.DefenseBonus;
            Amount = amount;
        }
    }
}

