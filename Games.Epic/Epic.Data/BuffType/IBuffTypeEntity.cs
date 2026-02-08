using System;

namespace Epic.Data.BuffType
{
    public interface IBuffFields
    {
        string Key { get; }
        string Name { get; }
        string ThumbnailUrl { get; }
        
        string HealthBonusExpression { get; }
        string AttackBonusExpression { get; }
        string DefenseBonusExpression { get; }
        string SpeedBonusExpression { get; }
        string MinDamageBonusExpression { get; }
        string MaxDamageBonusExpression { get; }
        string HealthBonusPercentageExpression { get; }
        string AttackBonusPercentageExpression { get; }
        string DefenseBonusPercentageExpression { get; }
        string SpeedBonusPercentageExpression { get; }
        string MinDamageBonusPercentageExpression { get; }
        string MaxDamageBonusPercentageExpression { get; }

        string VampirePercentageExpression { get; }

        bool Paralyzed { get; }
        bool Stunned { get; }
        bool VampireCanResurrect { get; }
        bool DeclinesWhenTakesDamage { get; }

        string HealsExpression { get; }
        string HealsPercentageExpression { get; }
        bool HealCanResurrect { get; }
        string TakesDamageMinExpression { get; }
        string TakesDamageMaxExpression { get; }

        string DamageReturnPercentageExpression { get; }
        string DamageReturnMaxRangeExpression { get; }

        bool Permanent { get; }
        string DurationExpression { get; }
    }

    public interface IBuffTypeEntity : IBuffFields
    {
        Guid Id { get; }
    }

    public class BuffFields : IBuffFields
    {
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
    }

    public class BuffTypeEntity : BuffFields, IBuffTypeEntity
    {
        public Guid Id { get; }

        private BuffTypeEntity(Guid id)
        {
            Id = id;
        }

        internal void FillFrom(IBuffFields fields)
        {
            Key = fields.Key;
            Name = fields.Name;
            ThumbnailUrl = fields.ThumbnailUrl;
            HealthBonusExpression = fields.HealthBonusExpression;
            AttackBonusExpression = fields.AttackBonusExpression;
            DefenseBonusExpression = fields.DefenseBonusExpression;
            SpeedBonusExpression = fields.SpeedBonusExpression;
            MinDamageBonusExpression = fields.MinDamageBonusExpression;
            MaxDamageBonusExpression = fields.MaxDamageBonusExpression;
            HealthBonusPercentageExpression = fields.HealthBonusPercentageExpression;
            AttackBonusPercentageExpression = fields.AttackBonusPercentageExpression;
            DefenseBonusPercentageExpression = fields.DefenseBonusPercentageExpression;
            SpeedBonusPercentageExpression = fields.SpeedBonusPercentageExpression;
            MinDamageBonusPercentageExpression = fields.MinDamageBonusPercentageExpression;
            MaxDamageBonusPercentageExpression = fields.MaxDamageBonusPercentageExpression;
            VampirePercentageExpression = fields.VampirePercentageExpression;
            Paralyzed = fields.Paralyzed;
            Stunned = fields.Stunned;
            VampireCanResurrect = fields.VampireCanResurrect;
            DeclinesWhenTakesDamage = fields.DeclinesWhenTakesDamage;
            HealsExpression = fields.HealsExpression;
            HealsPercentageExpression = fields.HealsPercentageExpression;
            HealCanResurrect = fields.HealCanResurrect;
            TakesDamageMinExpression = fields.TakesDamageMinExpression;
            TakesDamageMaxExpression = fields.TakesDamageMaxExpression;
            DamageReturnPercentageExpression = fields.DamageReturnPercentageExpression;
            DamageReturnMaxRangeExpression = fields.DamageReturnMaxRangeExpression;
            Permanent = fields.Permanent;
            DurationExpression = fields.DurationExpression;
        }

        public static BuffTypeEntity FromFields(Guid id, IBuffFields fields)
        {
            var entity = new BuffTypeEntity(id);
            entity.FillFrom(fields);
            return entity;
        }
    }
}
