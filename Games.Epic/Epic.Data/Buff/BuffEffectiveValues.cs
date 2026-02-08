namespace Epic.Data.Buff
{
    /// <summary>
    /// Evaluated effective values of a buff instance (result of evaluating BuffType expressions with a variables map).
    /// </summary>
    public class BuffEffectiveValues : IBuffEffectiveValues
    {
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

        /// <summary>Creates effective values from any source implementing IBuffEffectiveValues (e.g. entity fields when loading a buff).</summary>
        public static BuffEffectiveValues From(IBuffEffectiveValues source)
        {
            if (source == null)
                return new BuffEffectiveValues();
            return new BuffEffectiveValues
            {
                HealthBonus = source.HealthBonus,
                AttackBonus = source.AttackBonus,
                DefenseBonus = source.DefenseBonus,
                SpeedBonus = source.SpeedBonus,
                MinDamageBonus = source.MinDamageBonus,
                MaxDamageBonus = source.MaxDamageBonus,
                HealthBonusPercentage = source.HealthBonusPercentage,
                AttackBonusPercentage = source.AttackBonusPercentage,
                DefenseBonusPercentage = source.DefenseBonusPercentage,
                SpeedBonusPercentage = source.SpeedBonusPercentage,
                MinDamageBonusPercentage = source.MinDamageBonusPercentage,
                MaxDamageBonusPercentage = source.MaxDamageBonusPercentage,
                Paralyzed = source.Paralyzed,
                Stunned = source.Stunned,
                VampirePercentage = source.VampirePercentage,
                VampireCanResurrect = source.VampireCanResurrect,
                DeclinesWhenTakesDamage = source.DeclinesWhenTakesDamage,
                Heals = source.Heals,
                HealsPercentage = source.HealsPercentage,
                HealCanResurrect = source.HealCanResurrect,
                TakesDamageMin = source.TakesDamageMin,
                TakesDamageMax = source.TakesDamageMax,
                DamageReturnPercentage = source.DamageReturnPercentage,
                DamageReturnMaxRange = source.DamageReturnMaxRange,
                Permanent = source.Permanent,
                Duration = source.Duration,
            };
        }
    }
}
