using System;
using System.Linq;

namespace Epic.Data.Artifact
{
    public interface IArtifactFields
    {
        Guid TypeId { get; }
        Guid HeroId { get; }
        int[] EquippedSlotsIndexes { get; }
    }
    
    public interface IArtifactEntity : IArtifactFields
    {
        Guid Id { get; }
    }
    
    public class ArtifactFields : IArtifactFields
    {
        public Guid TypeId { get; set; }
        public Guid HeroId { get; set; }
        public int[] EquippedSlotsIndexes { get; set; } = Array.Empty<int>();
    }
    
    public class ArtifactEntity : ArtifactFields, IArtifactEntity
    {
        public Guid Id { get; }

        private ArtifactEntity(Guid id)
        {
            Id = id;
        }

        internal void FillFrom(IArtifactFields fields)
        {
            TypeId = fields.TypeId;
            HeroId = fields.HeroId;
            EquippedSlotsIndexes = fields.EquippedSlotsIndexes?.ToArray() ?? Array.Empty<int>();
        }

        public static ArtifactEntity FromFields(Guid id, IArtifactFields fields)
        {
            var entity = new ArtifactEntity(id);
            entity.FillFrom(fields);
            return entity;
        }
    }
}