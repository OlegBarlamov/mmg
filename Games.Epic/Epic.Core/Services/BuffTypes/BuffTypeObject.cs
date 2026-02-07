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
        public int HealthBonus { get; set; }
        public int AttackBonus { get; set; }
        public int DefenseBonus { get; set; }
        public int SpeedBonus { get; set; }
        public int MinDamageBonus { get; set; }
        public int MaxDamageBonus { get; set; }
        public int HealthBonusPercentage { get; set; }
        public int AttackBonusPercentage { get; set; }
        public int DefenseBonusPercentage { get; set; }
        public int SpeedBonusPercentage { get; set; }
        public int MinDamageBonusPercentage { get; set; }
        public int MaxDamageBonusPercentage { get; set; }
        public bool Paralyzed { get; set; }
        public bool Stunned { get; set; }
        public int VampirePercentage { get; set; }
        public bool VampireCanResurrect { get; set; }
        public bool DeclinesWhenTakesDamage { get; set; }
        public int Heals { get; set; }
        public int HealsPercentage { get; set; }
        public bool HealCanResurrect { get; set; }
        public int TakesDamageMin { get; set; }
        public int TakesDamageMax { get; set; }
        public int DamageReturnPercentage { get; set; }
        public int DamageReturnMaxRange { get; set; }
        public bool Permanent { get; set; }
        public int Duration { get; set; }

        private MutableBuffTypeObject() { }

        public static MutableBuffTypeObject FromEntity(IBuffTypeEntity entity)
        {
            return new MutableBuffTypeObject
            {
                Id = entity.Id,
                Key = entity.Key,
                Name = entity.Name,
                ThumbnailUrl = entity.ThumbnailUrl,
                HealthBonus = entity.HealthBonus,
                AttackBonus = entity.AttackBonus,
                DefenseBonus = entity.DefenseBonus,
                SpeedBonus = entity.SpeedBonus,
                MinDamageBonus = entity.MinDamageBonus,
                MaxDamageBonus = entity.MaxDamageBonus,
                HealthBonusPercentage = entity.HealthBonusPercentage,
                AttackBonusPercentage = entity.AttackBonusPercentage,
                DefenseBonusPercentage = entity.DefenseBonusPercentage,
                SpeedBonusPercentage = entity.SpeedBonusPercentage,
                MinDamageBonusPercentage = entity.MinDamageBonusPercentage,
                MaxDamageBonusPercentage = entity.MaxDamageBonusPercentage,
                Paralyzed = entity.Paralyzed,
                Stunned = entity.Stunned,
                VampirePercentage = entity.VampirePercentage,
                VampireCanResurrect = entity.VampireCanResurrect,
                DeclinesWhenTakesDamage = entity.DeclinesWhenTakesDamage,
                Heals = entity.Heals,
                HealsPercentage = entity.HealsPercentage,
                HealCanResurrect = entity.HealCanResurrect,
                TakesDamageMin = entity.TakesDamageMin,
                TakesDamageMax = entity.TakesDamageMax,
                DamageReturnPercentage = entity.DamageReturnPercentage,
                DamageReturnMaxRange = entity.DamageReturnMaxRange,
                Permanent = entity.Permanent,
                Duration = entity.Duration,
            };
        }

        public IBuffTypeEntity ToEntity()
        {
            return BuffTypeEntity.FromFields(Id, this);
        }
    }
}

