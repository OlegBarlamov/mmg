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
        public int TakesDamageMin { get; set; }
        public int TakesDamageMax { get; set; }
        public int DamageReturnPercentage { get; set; }
        public int DamageReturnMaxRange { get; set; }

        private MutableEffectTypeObject() { }

        public static MutableEffectTypeObject FromEntity(IEffectTypeEntity entity)
        {
            return new MutableEffectTypeObject
            {
                Id = entity.Id,
                Key = entity.Key,
                Name = entity.Name,
                SpriteUrl = entity.SpriteUrl,
                TakesDamageMin = entity.TakesDamageMin,
                TakesDamageMax = entity.TakesDamageMax,
                DamageReturnPercentage = entity.DamageReturnPercentage,
                DamageReturnMaxRange = entity.DamageReturnMaxRange,
            };
        }

        public IEffectTypeEntity ToEntity()
        {
            return EffectTypeEntity.FromFields(Id, this);
        }
    }
}
