using System;

namespace Epic.Data.BuffType
{
    public interface IBuffFields
    {
        string Key { get; }
        string Name { get; }
        string ThumbnailUrl { get; }
        int HealthBonus { get; }
        int AttackBonus { get; }
        int DefenseBonus { get; }
        int SpeedBonus { get; }
        int MinDamageBonus { get; }
        int MaxDamageBonus { get; }
        int HealthBonusPercentage { get; }
        int AttackBonusPercentage  { get; }
        int DefenseBonusPercentage { get; }
        int SpeedBonusPercentage { get; }
        int MinDamageBonusPercentage { get; }
        int MaxDamageBonusPercentage { get; }
        
        bool Paralyzed { get; }
        int VampirePercentage { get; }
        bool VampireCanResurrect { get; }
        bool DeclinesWhenTakesDamage { get; }
        
        bool Permanent { get; }
        int Duration { get; }
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
        public int VampirePercentage { get; set; }
        public bool VampireCanResurrect { get; set; }
        public bool DeclinesWhenTakesDamage { get; set; }
        public bool Permanent { get; set; }
        public int Duration { get; set; }
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
            VampirePercentage = fields.VampirePercentage;
            VampireCanResurrect = fields.VampireCanResurrect;
            DeclinesWhenTakesDamage = fields.DeclinesWhenTakesDamage;
            Permanent = fields.Permanent;
            Duration = fields.Duration;
        }

        public static BuffTypeEntity FromFields(Guid id, IBuffFields fields)
        {
            var entity = new BuffTypeEntity(id);
            entity.FillFrom(fields);
            return entity;
        }
    }
}