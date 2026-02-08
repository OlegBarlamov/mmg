using System;
using Epic.Core.Services.BuffTypes;
using Epic.Data.Buff;

namespace Epic.Core.Services.Buffs
{
    public class MutableBuffObject : IBuffObject, IBuffEntityFields
    {
        public Guid Id { get; set; }
        public Guid BuffTypeId { get; set; }
        public Guid TargetBattleUnitId { get; set; }
        public int DurationRemaining { get; set; }

        public IBuffTypeObject BuffType { get; set; }

        /// <summary>Effective values: from entity when loaded, or from evaluation when created. Never null after FromEntity/Create.</summary>
        private readonly IBuffEffectiveValues _effectiveValues;

        private static int GetInt(IBuffEffectiveValues v, Func<IBuffEffectiveValues, int> getter) => v != null ? getter(v) : 0;
        private static bool GetBool(IBuffEffectiveValues v, Func<IBuffEffectiveValues, bool> getter) => v != null && getter(v);

        public int HealthBonus => GetInt(_effectiveValues, x => x.HealthBonus);
        public int AttackBonus => GetInt(_effectiveValues, x => x.AttackBonus);
        public int DefenseBonus => GetInt(_effectiveValues, x => x.DefenseBonus);
        public int SpeedBonus => GetInt(_effectiveValues, x => x.SpeedBonus);
        public int MinDamageBonus => GetInt(_effectiveValues, x => x.MinDamageBonus);
        public int MaxDamageBonus => GetInt(_effectiveValues, x => x.MaxDamageBonus);
        public int HealthBonusPercentage => GetInt(_effectiveValues, x => x.HealthBonusPercentage);
        public int AttackBonusPercentage => GetInt(_effectiveValues, x => x.AttackBonusPercentage);
        public int DefenseBonusPercentage => GetInt(_effectiveValues, x => x.DefenseBonusPercentage);
        public int SpeedBonusPercentage => GetInt(_effectiveValues, x => x.SpeedBonusPercentage);
        public int MinDamageBonusPercentage => GetInt(_effectiveValues, x => x.MinDamageBonusPercentage);
        public int MaxDamageBonusPercentage => GetInt(_effectiveValues, x => x.MaxDamageBonusPercentage);
        public bool Paralyzed => GetBool(_effectiveValues, x => x.Paralyzed);
        public bool Stunned => GetBool(_effectiveValues, x => x.Stunned);
        public int VampirePercentage => GetInt(_effectiveValues, x => x.VampirePercentage);
        public bool VampireCanResurrect => GetBool(_effectiveValues, x => x.VampireCanResurrect);
        public bool DeclinesWhenTakesDamage => GetBool(_effectiveValues, x => x.DeclinesWhenTakesDamage);
        public int Heals => GetInt(_effectiveValues, x => x.Heals);
        public int HealsPercentage => GetInt(_effectiveValues, x => x.HealsPercentage);
        public bool HealCanResurrect => GetBool(_effectiveValues, x => x.HealCanResurrect);
        public int TakesDamageMin => GetInt(_effectiveValues, x => x.TakesDamageMin);
        public int TakesDamageMax => GetInt(_effectiveValues, x => x.TakesDamageMax);
        public int DamageReturnPercentage => GetInt(_effectiveValues, x => x.DamageReturnPercentage);
        public int DamageReturnMaxRange => GetInt(_effectiveValues, x => x.DamageReturnMaxRange);
        public bool Permanent => GetBool(_effectiveValues, x => x.Permanent);
        public int Duration => GetInt(_effectiveValues, x => x.Duration);

        private MutableBuffObject(IBuffEffectiveValues effectiveValues)
        {
            _effectiveValues = effectiveValues;
        }

        /// <summary>When loading, entity already has all effective values; when creating, pass evaluated effectiveValues.</summary>
        public static MutableBuffObject FromEntity(IBuffEntity entity, BuffEffectiveValues effectiveValues = null)
        {
            var source = effectiveValues ?? (IBuffEffectiveValues)entity;
            return new MutableBuffObject(source)
            {
                Id = entity.Id,
                BuffTypeId = entity.BuffTypeId,
                TargetBattleUnitId = entity.TargetBattleUnitId,
                DurationRemaining = entity.DurationRemaining,
                BuffType = null,
            };
        }

        public IBuffEntity ToEntity()
        {
            var fields = new BuffEntityFields
            {
                BuffTypeId = BuffTypeId,
                TargetBattleUnitId = TargetBattleUnitId,
                DurationRemaining = DurationRemaining,
            };
            fields.SetEffectiveValues(_effectiveValues);
            return BuffEntity.FromFields(Id, fields);
        }
    }
}

