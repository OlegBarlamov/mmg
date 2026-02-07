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
        public int Heals { get; set; }
        public int HealsPercentage { get; set; }
        public bool HealCanResurrect { get; set; }

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
                Heals = entity.Heals,
                HealsPercentage = entity.HealsPercentage,
                HealCanResurrect = entity.HealCanResurrect,
            };
        }

        public IEffectTypeEntity ToEntity()
        {
            return EffectTypeEntity.FromFields(Id, this);
        }
    }
}
