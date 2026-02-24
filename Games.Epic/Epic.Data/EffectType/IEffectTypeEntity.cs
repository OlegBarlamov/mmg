using System;
using JetBrains.Annotations;

namespace Epic.Data.EffectType
{
    public interface IEffectTypeFields
    {
        string Key { get; }
        string Name { get; }
        
        string TakesDamageMinExpression { get; }
        string TakesDamageMaxExpression { get; }
        string HealsExpression { get; }
        string HealsPercentageExpression { get; }
        bool HealCanResurrect { get; }
        EffectAnimation Animation { get; }
        int AnimationTimeMs { get; }

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
        public string TakesDamageMinExpression { get; set; }
        public string TakesDamageMaxExpression { get; set; }
        public string HealsExpression { get; set; }
        public string HealsPercentageExpression { get; set; }
        public bool HealCanResurrect { get; set; }
        public EffectAnimation Animation { get; set; }
        public int AnimationTimeMs { get; set; }
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
            TakesDamageMinExpression = fields.TakesDamageMinExpression;
            TakesDamageMaxExpression = fields.TakesDamageMaxExpression;
            HealsExpression = fields.HealsExpression;
            HealsPercentageExpression = fields.HealsPercentageExpression;
            HealCanResurrect = fields.HealCanResurrect;
            Animation = fields.Animation;
            AnimationTimeMs = fields.AnimationTimeMs;
        }

        public static EffectTypeEntity FromFields(Guid id, IEffectTypeFields fields)
        {
            var entity = new EffectTypeEntity(id);
            entity.FillFrom(fields);
            return entity;
        }
    }
}