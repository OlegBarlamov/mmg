using System;
using System.Linq;
using Epic.Core.Services.Magic;
using Epic.Data.Buff;
using Epic.Data.EffectType;

namespace Epic.Server.Resources
{
    public class MagicResource
    {
        public Guid MagicTypeId { get; }
        public string Name { get; }
        public string Description { get; }
        public string ThumbnailUrl { get; }
        public int MannaCost { get; }
        public string CastTargetType { get; }
        public int EffectRadius { get; }
        public MagicBuffResource[] ApplyBuffs { get; }
        public MagicEffectResource[] ApplyEffects { get; }

        public MagicResource(IMagicObject magic)
        {
            if (magic == null) throw new ArgumentNullException(nameof(magic));
            MagicTypeId = magic.MagicTypeId;
            var type = magic.MagicType;
            Name = type?.Name;
            Description = type?.Description;
            ThumbnailUrl = type?.ThumbnailUrl;
            MannaCost = type?.MannaCost ?? 0;
            CastTargetType = type?.CastTargetType.ToString();
            EffectRadius = type?.EffectRadius ?? 0;
            ApplyBuffs = magic.ApplyBuffs?
                .Select(b => new MagicBuffResource(b.BuffType?.Name, b.BuffType?.Key, b.BuffType?.DurationExpression, b.EffectiveValues))
                .ToArray() ?? Array.Empty<MagicBuffResource>();
            ApplyEffects = magic.ApplyEffects?
                .Select(e => new MagicEffectResource(
                    e.EffectType?.Name, e.EffectType?.Key,
                    e.TakesDamageMin, e.TakesDamageMax, e.Heals, e.HealsPercentage,
                    e.EffectType?.TakesDamageMinExpression, e.EffectType?.TakesDamageMaxExpression,
                    e.EffectType?.HealsExpression, e.EffectType?.HealsPercentageExpression,
                    e.Animation, e.AnimationTimeMs))
                .ToArray() ?? Array.Empty<MagicEffectResource>();
        }
    }

    public class MagicBuffResource
    {
        public string Name { get; }
        public string Key { get; }
        public string DurationExpression { get; }
        public int Duration { get; }
        public bool Permanent { get; }
        public int HealthBonus { get; }
        public int AttackBonus { get; }
        public int DefenseBonus { get; }
        public int SpeedBonus { get; }
        public int MinDamageBonus { get; }
        public int MaxDamageBonus { get; }
        public int HealthBonusPercentage { get; }
        public int AttackBonusPercentage { get; }
        public int DefenseBonusPercentage { get; }
        public int SpeedBonusPercentage { get; }
        public int MinDamageBonusPercentage { get; }
        public int MaxDamageBonusPercentage { get; }
        public bool Paralyzed { get; }
        public bool Stunned { get; }
        public int VampirePercentage { get; }
        public bool VampireCanResurrect { get; }
        public bool DeclinesWhenTakesDamage { get; }
        public int Heals { get; }
        public int HealsPercentage { get; }
        public bool HealCanResurrect { get; }
        public int TakesDamageMin { get; }
        public int TakesDamageMax { get; }
        public int DamageReturnPercentage { get; }
        public int DamageReturnMaxRange { get; }

        public MagicBuffResource(string name, string key, string durationExpression, IBuffEffectiveValues effectiveValues)
        {
            Name = name;
            Key = key;
            DurationExpression = durationExpression;
            var ev = effectiveValues;
            Duration = ev?.Duration ?? 0;
            Permanent = ev?.Permanent ?? false;
            HealthBonus = ev?.HealthBonus ?? 0;
            AttackBonus = ev?.AttackBonus ?? 0;
            DefenseBonus = ev?.DefenseBonus ?? 0;
            SpeedBonus = ev?.SpeedBonus ?? 0;
            MinDamageBonus = ev?.MinDamageBonus ?? 0;
            MaxDamageBonus = ev?.MaxDamageBonus ?? 0;
            HealthBonusPercentage = ev?.HealthBonusPercentage ?? 0;
            AttackBonusPercentage = ev?.AttackBonusPercentage ?? 0;
            DefenseBonusPercentage = ev?.DefenseBonusPercentage ?? 0;
            SpeedBonusPercentage = ev?.SpeedBonusPercentage ?? 0;
            MinDamageBonusPercentage = ev?.MinDamageBonusPercentage ?? 0;
            MaxDamageBonusPercentage = ev?.MaxDamageBonusPercentage ?? 0;
            Paralyzed = ev?.Paralyzed ?? false;
            Stunned = ev?.Stunned ?? false;
            VampirePercentage = ev?.VampirePercentage ?? 0;
            VampireCanResurrect = ev?.VampireCanResurrect ?? false;
            DeclinesWhenTakesDamage = ev?.DeclinesWhenTakesDamage ?? false;
            Heals = ev?.Heals ?? 0;
            HealsPercentage = ev?.HealsPercentage ?? 0;
            HealCanResurrect = ev?.HealCanResurrect ?? false;
            TakesDamageMin = ev?.TakesDamageMin ?? 0;
            TakesDamageMax = ev?.TakesDamageMax ?? 0;
            DamageReturnPercentage = ev?.DamageReturnPercentage ?? 0;
            DamageReturnMaxRange = ev?.DamageReturnMaxRange ?? 0;
        }
    }

    public class MagicEffectResource
    {
        public string Name { get; }
        public string Key { get; }
        public int TakesDamageMin { get; }
        public int TakesDamageMax { get; }
        public int Heals { get; }
        public int HealsPercentage { get; }
        public string TakesDamageMinExpression { get; }
        public string TakesDamageMaxExpression { get; }
        public string HealsExpression { get; }
        public string HealsPercentageExpression { get; }
        public EffectAnimation Animation { get; }
        public int AnimationTimeMs { get; }

        public MagicEffectResource(string name, string key, int takesDamageMin, int takesDamageMax, int heals, int healsPercentage,
            string takesDamageMinExpression, string takesDamageMaxExpression, string healsExpression, string healsPercentageExpression,
            EffectAnimation animation = EffectAnimation.None, int animationTimeMs = 0)
        {
            Name = name;
            Key = key;
            TakesDamageMin = takesDamageMin;
            TakesDamageMax = takesDamageMax;
            Heals = heals;
            HealsPercentage = healsPercentage;
            TakesDamageMinExpression = takesDamageMinExpression;
            TakesDamageMaxExpression = takesDamageMaxExpression;
            HealsExpression = healsExpression;
            HealsPercentageExpression = healsPercentageExpression;
            Animation = animation;
            AnimationTimeMs = animationTimeMs;
        }
    }
}
