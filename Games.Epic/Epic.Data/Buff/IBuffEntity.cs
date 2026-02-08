using System;

namespace Epic.Data.Buff
{
    public interface IBuffEntityFields : IBuffEffectiveValues
    {
        Guid BuffTypeId { get; }
        Guid TargetBattleUnitId { get; }
        int DurationRemaining { get; }
    }

    public interface IBuffEntity : IBuffEntityFields
    {
        Guid Id { get; set; }
    }

    public class BuffEntityFields : IBuffEntityFields
    {
        public Guid BuffTypeId { get; set; }
        public Guid TargetBattleUnitId { get; set; }
        public int DurationRemaining { get; set; }

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

        /// <summary>Copies effective values from the evaluated result (e.g. when creating or updating a buff).</summary>
        public void SetEffectiveValues(IBuffEffectiveValues values)
        {
            if (values == null) return;
            HealthBonus = values.HealthBonus;
            AttackBonus = values.AttackBonus;
            DefenseBonus = values.DefenseBonus;
            SpeedBonus = values.SpeedBonus;
            MinDamageBonus = values.MinDamageBonus;
            MaxDamageBonus = values.MaxDamageBonus;
            HealthBonusPercentage = values.HealthBonusPercentage;
            AttackBonusPercentage = values.AttackBonusPercentage;
            DefenseBonusPercentage = values.DefenseBonusPercentage;
            SpeedBonusPercentage = values.SpeedBonusPercentage;
            MinDamageBonusPercentage = values.MinDamageBonusPercentage;
            MaxDamageBonusPercentage = values.MaxDamageBonusPercentage;
            Paralyzed = values.Paralyzed;
            Stunned = values.Stunned;
            VampirePercentage = values.VampirePercentage;
            VampireCanResurrect = values.VampireCanResurrect;
            DeclinesWhenTakesDamage = values.DeclinesWhenTakesDamage;
            Heals = values.Heals;
            HealsPercentage = values.HealsPercentage;
            HealCanResurrect = values.HealCanResurrect;
            TakesDamageMin = values.TakesDamageMin;
            TakesDamageMax = values.TakesDamageMax;
            DamageReturnPercentage = values.DamageReturnPercentage;
            DamageReturnMaxRange = values.DamageReturnMaxRange;
            Permanent = values.Permanent;
            Duration = values.Duration;
        }
    }

    public class BuffEntity : BuffEntityFields, IBuffEntity
    {
        public Guid Id { get; set; }

        internal void FillFrom(IBuffEntityFields fields)
        {
            BuffTypeId = fields.BuffTypeId;
            TargetBattleUnitId = fields.TargetBattleUnitId;
            DurationRemaining = fields.DurationRemaining;
            HealthBonus = fields.HealthBonus;
            AttackBonus = fields.AttackBonus;
            DefenseBonus = fields.DefenseBonus;
            SpeedBonus = fields.SpeedBonus;
            MinDamageBonus = fields.MinDamageBonus;
            MaxDamageBonus = fields.MaxDamageBonus;
            HealthBonusPercentage = fields.HealthBonusPercentage;
            AttackBonusPercentage = fields.AttackBonusPercentage;
            DefenseBonusPercentage = fields.DefenseBonusPercentage;
            SpeedBonusPercentage = fields.SpeedBonusPercentage;
            MinDamageBonusPercentage = fields.MinDamageBonusPercentage;
            MaxDamageBonusPercentage = fields.MaxDamageBonusPercentage;
            Paralyzed = fields.Paralyzed;
            Stunned = fields.Stunned;
            VampirePercentage = fields.VampirePercentage;
            VampireCanResurrect = fields.VampireCanResurrect;
            DeclinesWhenTakesDamage = fields.DeclinesWhenTakesDamage;
            Heals = fields.Heals;
            HealsPercentage = fields.HealsPercentage;
            HealCanResurrect = fields.HealCanResurrect;
            TakesDamageMin = fields.TakesDamageMin;
            TakesDamageMax = fields.TakesDamageMax;
            DamageReturnPercentage = fields.DamageReturnPercentage;
            DamageReturnMaxRange = fields.DamageReturnMaxRange;
            Permanent = fields.Permanent;
            Duration = fields.Duration;
        }

        public static BuffEntity FromFields(Guid id, IBuffEntityFields fields)
        {
            var entity = new BuffEntity { Id = id };
            entity.FillFrom(fields);
            return entity;
        }
    }
}