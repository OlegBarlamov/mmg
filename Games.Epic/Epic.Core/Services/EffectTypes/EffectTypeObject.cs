using System;
using Epic.Data.EffectType;

namespace Epic.Core.Services.EffectTypes
{
    public class MutableEffectTypeObject : IEffectTypeObject, IEffectTypeFields
    {
        public Guid Id { get; set; }
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

        private MutableEffectTypeObject() { }

        public static MutableEffectTypeObject FromEntity(IEffectTypeEntity entity)
        {
            return new MutableEffectTypeObject
            {
                Id = entity.Id,
                Key = entity.Key,
                Name = entity.Name,
                SpriteUrl = entity.SpriteUrl,
                TakesDamageMinExpression = entity.TakesDamageMinExpression,
                TakesDamageMaxExpression = entity.TakesDamageMaxExpression,
                HealsExpression = entity.HealsExpression,
                HealsPercentageExpression = entity.HealsPercentageExpression,
                HealCanResurrect = entity.HealCanResurrect,
                Animation = entity.Animation,
                AnimationTimeMs = entity.AnimationTimeMs,
            };
        }

        public IEffectTypeEntity ToEntity()
        {
            return EffectTypeEntity.FromFields(Id, this);
        }
    }
}
