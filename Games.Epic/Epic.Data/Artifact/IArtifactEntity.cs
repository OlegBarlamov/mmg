using System;

namespace Epic.Data.Artifact
{
    public interface IArtifactEntityFields
    {
        Guid TypeId { get; }
        Guid HeroId { get; }
        int EquippedSlotIndex { get; }
    }
    
    public interface IArtifactEntity : IArtifactEntityFields
    {
        Guid Id { get; }
    }
    
    public class ArtifactEntityFields : IArtifactEntityFields
    {
        public Guid TypeId { get; set; }
        public Guid HeroId { get; set; }
        public int EquippedSlotIndex { get; set; }
    }
    
    public class ArtifactEntity : ArtifactEntityFields, IArtifactEntity
    {
        public Guid Id { get; }

        private ArtifactEntity(Guid id)
        {
            Id = id;
        }

        internal void FillFrom(IArtifactEntityFields fields)
        {
            TypeId = fields.TypeId;
            HeroId = fields.HeroId;
            EquippedSlotIndex = fields.EquippedSlotIndex;
        }

        public static ArtifactEntity FromFields(Guid id, IArtifactEntityFields fields)
        {
            var entity = new ArtifactEntity(id);
            entity.FillFrom(fields);
            return entity;
        }
    }
}