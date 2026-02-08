using System;
using Epic.Data.BuffType;

namespace Epic.Core.Services.BuffTypes
{
    public class MutableBuffTypeObject : IBuffTypeObject, IBuffFields
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string ThumbnailUrl { get; set; }
        public string HealthBonusExpression { get; set; }
        public string AttackBonusExpression { get; set; }
        public string DefenseBonusExpression { get; set; }
        public string SpeedBonusExpression { get; set; }
        public string MinDamageBonusExpression { get; set; }
        public string MaxDamageBonusExpression { get; set; }
        public string HealthBonusPercentageExpression { get; set; }
        public string AttackBonusPercentageExpression { get; set; }
        public string DefenseBonusPercentageExpression { get; set; }
        public string SpeedBonusPercentageExpression { get; set; }
        public string MinDamageBonusPercentageExpression { get; set; }
        public string MaxDamageBonusPercentageExpression { get; set; }
        public string VampirePercentageExpression { get; set; }
        public bool Paralyzed { get; set; }
        public bool Stunned { get; set; }
        public bool VampireCanResurrect { get; set; }
        public bool DeclinesWhenTakesDamage { get; set; }
        public string HealsExpression { get; set; }
        public string HealsPercentageExpression { get; set; }
        public bool HealCanResurrect { get; set; }
        public string TakesDamageMinExpression { get; set; }
        public string TakesDamageMaxExpression { get; set; }
        public string DamageReturnPercentageExpression { get; set; }
        public string DamageReturnMaxRangeExpression { get; set; }
        public bool Permanent { get; set; }
        public string DurationExpression { get; set; }

        private MutableBuffTypeObject() { }

        public static MutableBuffTypeObject FromEntity(IBuffTypeEntity entity)
        {
            return new MutableBuffTypeObject
            {
                Id = entity.Id,
                Key = entity.Key,
                Name = entity.Name,
                ThumbnailUrl = entity.ThumbnailUrl,
                HealthBonusExpression = entity.HealthBonusExpression,
                AttackBonusExpression = entity.AttackBonusExpression,
                DefenseBonusExpression = entity.DefenseBonusExpression,
                SpeedBonusExpression = entity.SpeedBonusExpression,
                MinDamageBonusExpression = entity.MinDamageBonusExpression,
                MaxDamageBonusExpression = entity.MaxDamageBonusExpression,
                HealthBonusPercentageExpression = entity.HealthBonusPercentageExpression,
                AttackBonusPercentageExpression = entity.AttackBonusPercentageExpression,
                DefenseBonusPercentageExpression = entity.DefenseBonusPercentageExpression,
                SpeedBonusPercentageExpression = entity.SpeedBonusPercentageExpression,
                MinDamageBonusPercentageExpression = entity.MinDamageBonusPercentageExpression,
                MaxDamageBonusPercentageExpression = entity.MaxDamageBonusPercentageExpression,
                VampirePercentageExpression = entity.VampirePercentageExpression,
                Paralyzed = entity.Paralyzed,
                Stunned = entity.Stunned,
                VampireCanResurrect = entity.VampireCanResurrect,
                DeclinesWhenTakesDamage = entity.DeclinesWhenTakesDamage,
                HealsExpression = entity.HealsExpression,
                HealsPercentageExpression = entity.HealsPercentageExpression,
                HealCanResurrect = entity.HealCanResurrect,
                TakesDamageMinExpression = entity.TakesDamageMinExpression,
                TakesDamageMaxExpression = entity.TakesDamageMaxExpression,
                DamageReturnPercentageExpression = entity.DamageReturnPercentageExpression,
                DamageReturnMaxRangeExpression = entity.DamageReturnMaxRangeExpression,
                Permanent = entity.Permanent,
                DurationExpression = entity.DurationExpression,
            };
        }

        public IBuffTypeEntity ToEntity()
        {
            return BuffTypeEntity.FromFields(Id, this);
        }
    }
}
