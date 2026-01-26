using System;
using Epic.Data.BuffType;

namespace Epic.Core.Services.BuffTypes
{
    public class MutableBuffTypeObject : IBuffTypeObject, IBuffFields
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
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
        public bool Stunned { get; set; }
        public int VampirePercentage { get; set; }
        public bool VampireCanResurrect { get; set; }
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
                Stunned = entity.Stunned,
                VampirePercentage = entity.VampirePercentage,
                VampireCanResurrect = entity.VampireCanResurrect,
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

