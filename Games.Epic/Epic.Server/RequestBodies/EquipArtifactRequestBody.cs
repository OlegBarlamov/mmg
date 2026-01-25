using System;

namespace Epic.Server.RequestBodies
{
    public class EquipArtifactRequestBody
    {
        public Guid ArtifactId { get; set; }
        public int[] EquippedSlotsIndexes { get; set; } = Array.Empty<int>();
    }
}

