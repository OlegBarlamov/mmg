using Epic.Data.EffectType;

namespace Epic.Data.Buff
{
    public interface IBuffEffectiveValues : IEffectTypeProperties
    {
        int HealthBonus { get; }
        int AttackBonus { get; }
        int DefenseBonus { get; }
        int SpeedBonus { get; }
        int MinDamageBonus { get; }
        int MaxDamageBonus { get; }
        int HealthBonusPercentage { get; }
        int AttackBonusPercentage { get; }
        int DefenseBonusPercentage { get; }
        int SpeedBonusPercentage { get; }
        int MinDamageBonusPercentage { get; }
        int MaxDamageBonusPercentage { get; }
        bool Paralyzed { get; }
        bool Stunned { get; }
        int VampirePercentage { get; }
        bool VampireCanResurrect { get; }
        bool DeclinesWhenTakesDamage { get; }
        int DamageReturnPercentage { get; }
        int DamageReturnMaxRange { get; }
        bool Permanent { get; }
        int Duration { get; }
    }
}
