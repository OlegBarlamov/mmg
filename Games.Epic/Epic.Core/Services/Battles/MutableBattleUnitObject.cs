using System;
using System.Collections.Generic;
using System.Linq;
using Epic.Core.Services.Buffs;
using Epic.Core.Services.Units;
using Epic.Data.BattleUnits;
using Epic.Data.Heroes;

namespace Epic.Core.Services.Battles
{
    public class MutableBattleUnitObject : IBattleUnitObject
    {
        public Guid Id { get; }
        public Guid BattleId { get; set; }
        public Guid PlayerUnitId { get; set; }
        public MutableGlobalUnitObject GlobalUnit { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public int PlayerIndex { get; set; }
        public int CurrentHealth { get; set; }
        public int InitialCount { get; set; }
        public int CurrentCount { get; set; }
        public bool Waited { get; set; }
        
        public IHeroStats HeroStats { get; set; }

        // Computed properties: base + hero bonus + buff bonuses
        public int MaxHealth => CalculateMaxHealth();
        public int CurrentAttack => CalculateCurrentAttack();
        public int CurrentDefense => CalculateCurrentDefense();

        public IReadOnlyList<AttackFunctionStateEntity> AttackFunctionsData { get; set; }
        public IReadOnlyList<IBuffObject> Buffs { get; set; } = Array.Empty<IBuffObject>();
        
        /// <summary>
        /// Whether the unit is paralyzed (cannot perform any actions).
        /// </summary>
        public bool IsParalyzed => Buffs?.Any(buff => buff.BuffType?.Paralyzed == true) ?? false;
        
        /// <summary>
        /// Whether the unit is stunned (cannot move but can still attack).
        /// </summary>
        public bool IsStunned => Buffs?.Any(buff => buff.BuffType?.Stunned == true) ?? false;
        
        /// <summary>
        /// Whether the unit can act this turn (not paralyzed).
        /// </summary>
        public bool CanAct => !IsParalyzed;

        IGlobalUnitObject IBattleUnitObject.GlobalUnit => GlobalUnit;

        private int CalculateMaxHealth()
        {
            var baseHealth = GlobalUnit?.UnitType?.Health ?? 0;
            var buffBonus = CalculateBuffHealthBonus();
            return baseHealth + buffBonus;
        }

        private int CalculateCurrentAttack()
        {
            var baseAttack = GlobalUnit?.UnitType?.Attack ?? 0;
            var heroBonus = HeroStats?.Attack ?? 0;
            var buffBonus = CalculateBuffAttackBonus();
            return baseAttack + heroBonus + buffBonus;
        }

        private int CalculateCurrentDefense()
        {
            var baseDefense = GlobalUnit?.UnitType?.Defense ?? 0;
            var heroBonus = HeroStats?.Defense ?? 0;
            var buffBonus = CalculateBuffDefenseBonus();
            return baseDefense + heroBonus + buffBonus;
        }

        private int CalculateBuffHealthBonus()
        {
            if (Buffs == null || Buffs.Count == 0)
                return 0;

            var flatBonus = Buffs.Where(b => b.BuffType != null).Sum(b => b.BuffType.HealthBonus);
            var percentageBonus = Buffs.Where(b => b.BuffType != null).Sum(b => b.BuffType.HealthBonusPercentage);
            
            var baseHealth = GlobalUnit?.UnitType?.Health ?? 0;
            return flatBonus + (baseHealth * percentageBonus / 100);
        }

        private int CalculateBuffAttackBonus()
        {
            if (Buffs == null || Buffs.Count == 0)
                return 0;

            var flatBonus = Buffs.Where(b => b.BuffType != null).Sum(b => b.BuffType.AttackBonus);
            var percentageBonus = Buffs.Where(b => b.BuffType != null).Sum(b => b.BuffType.AttackBonusPercentage);
            
            var baseAttack = GlobalUnit?.UnitType?.Attack ?? 0;
            return flatBonus + (baseAttack * percentageBonus / 100);
        }

        private int CalculateBuffDefenseBonus()
        {
            if (Buffs == null || Buffs.Count == 0)
                return 0;

            var flatBonus = Buffs.Where(b => b.BuffType != null).Sum(b => b.BuffType.DefenseBonus);
            var percentageBonus = Buffs.Where(b => b.BuffType != null).Sum(b => b.BuffType.DefenseBonusPercentage);
            
            var baseDefense = GlobalUnit?.UnitType?.Defense ?? 0;
            return flatBonus + (baseDefense * percentageBonus / 100);
        }

        /// <summary>
        /// Calculates the effective MinDamage for an attack, applying buff bonuses.
        /// </summary>
        public int GetEffectiveMinDamage(int baseMinDamage)
        {
            if (Buffs == null || Buffs.Count == 0)
                return Math.Max(0, baseMinDamage);

            var flatBonus = Buffs.Where(b => b.BuffType != null).Sum(b => b.BuffType.MinDamageBonus);
            var percentageBonus = Buffs.Where(b => b.BuffType != null).Sum(b => b.BuffType.MinDamageBonusPercentage);
            
            var result = baseMinDamage + flatBonus + (baseMinDamage * percentageBonus / 100);
            return Math.Max(0, result);
        }

        /// <summary>
        /// Calculates the effective MaxDamage for an attack, applying buff bonuses.
        /// MaxDamage is guaranteed to be at least equal to MinDamage.
        /// </summary>
        public int GetEffectiveMaxDamage(int baseMinDamage, int baseMaxDamage)
        {
            var effectiveMin = GetEffectiveMinDamage(baseMinDamage);
            
            if (Buffs == null || Buffs.Count == 0)
                return Math.Max(effectiveMin, baseMaxDamage);

            var flatBonus = Buffs.Where(b => b.BuffType != null).Sum(b => b.BuffType.MaxDamageBonus);
            var percentageBonus = Buffs.Where(b => b.BuffType != null).Sum(b => b.BuffType.MaxDamageBonusPercentage);
            
            var result = baseMaxDamage + flatBonus + (baseMaxDamage * percentageBonus / 100);
            
            // MaxDamage cannot be less than MinDamage
            return Math.Max(effectiveMin, result);
        }

        private MutableBattleUnitObject(Guid id)
        {
            Id = id;
        }
        
        public static MutableBattleUnitObject FromEntity(IBattleUnitEntity entity)
        {
            return new MutableBattleUnitObject(entity.Id)
            {
                BattleId = entity.BattleId,
                PlayerUnitId = entity.GlobalUnitId,
                Column = entity.Column,
                Row = entity.Row,
                PlayerIndex = entity.PlayerIndex,
                CurrentHealth = entity.CurrentHealth,
                InitialCount = entity.InitialCount,
                CurrentCount = entity.CurrentCount,
                Waited = entity.Waited,
            };
        }
        
        public static MutableBattleUnitObject CopyFrom(IBattleUnitObject x)
        {
            var entity = x.ToEntity();
            var battleUnitObject = FromEntity(entity);
            battleUnitObject.GlobalUnit = MutableGlobalUnitObject.CopyFrom(x.GlobalUnit);
            battleUnitObject.AttackFunctionsData = x.AttackFunctionsData;
            battleUnitObject.Buffs = x.Buffs;
            battleUnitObject.HeroStats = x.HeroStats;
            return battleUnitObject;
        }

        public IBattleUnitEntity ToEntity()
        {
            return DefaultBattleUnitsService.BattleUnitEntity.FromBattleUnitObject(this);
        }
    }
}