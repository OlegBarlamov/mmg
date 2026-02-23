using System;

namespace Epic.Data.MagicType
{
    public interface IMagicTypeFields
    {
        string Key { get; }
        string Name { get; }
        string ThumbnailUrl { get; }
        int Value { get; }
        int MannaCost { get; }
        CastTargetType CastTargetType { get; }
        int EffectRadius { get; }
        Guid[] ApplyBuffsIds { get; }
        Guid[] ApplyEffectsIds { get; }
    }
    
    public interface IMagicTypeEntity : IMagicTypeFields
    {
        Guid Id { get; set; }
    }

    public class MagicTypeFields : IMagicTypeFields
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string ThumbnailUrl { get; set; }
        public int Value { get; set; }
        public int MannaCost { get; set; }
        public CastTargetType CastTargetType { get; set; }
        public int EffectRadius { get; set; }
        public Guid[] ApplyBuffsIds { get; set; }
        public Guid[] ApplyEffectsIds { get; set; }
    }

    public class MagicTypeEntity : MagicTypeFields, IMagicTypeEntity
    {
        public Guid Id { get; set; }

        internal void FillFrom(IMagicTypeFields fields)
        {
            Key = fields.Key;
            Name = fields.Name;
            ThumbnailUrl = fields.ThumbnailUrl;
            Value = fields.Value;
            MannaCost = fields.MannaCost;
            CastTargetType = fields.CastTargetType;
            EffectRadius = fields.EffectRadius;
            ApplyBuffsIds = fields.ApplyBuffsIds;
            ApplyEffectsIds = fields.ApplyEffectsIds;
        }

        public static MagicTypeEntity FromFields(Guid id, IMagicTypeFields fields)
        {
            var entity = new MagicTypeEntity { Id = id };
            entity.FillFrom(fields);
            return entity;
        }
    }
}