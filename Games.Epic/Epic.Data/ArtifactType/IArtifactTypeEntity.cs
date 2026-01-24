using System;
using System.Linq;
using Epic.Data.Artifacts;

namespace Epic.Data.ArtifactType
{
    public interface IArtifactTypeEntityFields
    {
        string Key { get; }
        string Name { get; }
        ArtifactSlot[] Slots { get; }
        int AttackBonus { get; }
        int DefenseBonus { get; }
    }
    
    public interface IArtifactTypeEntity : IArtifactTypeEntityFields
    {
        Guid Id { get; }
    }
    
    public class ArtifactTypeEntityFields : IArtifactTypeEntityFields
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public ArtifactSlot[] Slots { get; set; } = Array.Empty<ArtifactSlot>();
        public int AttackBonus { get; set; }
        public int DefenseBonus { get; set; }
    }
    
    public class ArtifactTypeEntity : ArtifactTypeEntityFields, IArtifactTypeEntity
    {
        public Guid Id { get; }

        private ArtifactTypeEntity(Guid id)
        {
            Id = id;
        }

        internal void FillFrom(IArtifactTypeEntityFields fields)
        {
            Key = fields.Key;
            Name = fields.Name;
            Slots = fields.Slots?.ToArray() ?? Array.Empty<ArtifactSlot>();
            AttackBonus = fields.AttackBonus;
            DefenseBonus = fields.DefenseBonus;
        }

        public static ArtifactTypeEntity FromFields(Guid id, IArtifactTypeEntityFields fields)
        {
            var entity = new ArtifactTypeEntity(id);
            entity.FillFrom(fields);
            return entity;
        }
    }
}