using System;
using JetBrains.Annotations;

namespace Epic.Data.EffectType
{
    public interface IEffectTypeProperties
    {
        int TakesDamageMin { get; }
        int TakesDamageMax { get; }
        
        int Heals { get; }
        int HealsPercentage { get; }
        bool HealCanResurrect { get; }
    }
    
    public interface IEffectTypeFields : IEffectTypeProperties
    {
        string Key { get; }
        string Name { get; }
        
        [CanBeNull]
        string SpriteUrl { get; }
    }
    
    public interface IEffectTypeEntity : IEffectTypeFields
    {
        Guid Id { get; }
    }

    public class EffectTypeFields : IEffectTypeFields
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string SpriteUrl { get; set; }
        public int TakesDamageMin { get; set; }
        public int TakesDamageMax { get; set; }
        public int Heals { get; set; }
        public int HealsPercentage { get; set; }
        public bool HealCanResurrect { get; set; }
    }

    public class EffectTypeEntity : EffectTypeFields, IEffectTypeEntity
    {
        public Guid Id { get; }

        private EffectTypeEntity(Guid id)
        {
            Id = id;
        }

        internal void FillFrom(IEffectTypeFields fields)
        {
            Key = fields.Key;
            Name = fields.Name;
            SpriteUrl = fields.SpriteUrl;
            TakesDamageMin = fields.TakesDamageMin;
            TakesDamageMax = fields.TakesDamageMax;
            Heals = fields.Heals;
            HealsPercentage = fields.HealsPercentage;
            HealCanResurrect = fields.HealCanResurrect;
        }

        public static EffectTypeEntity FromFields(Guid id, IEffectTypeFields fields)
        {
            var entity = new EffectTypeEntity(id);
            entity.FillFrom(fields);
            return entity;
        }
    }
}