using System;
using System.Collections.Generic;
using System.Linq;
using Epic.Core.Services.Battles;
using Epic.Core.Services.Buffs;
using Epic.Data.Effect;

namespace Epic.Logic.Battle
{
    public static class BattleUnitBuffExtensions
    {
        public static bool IsParalyzed(this IBattleUnitObject unit)
        {
            return unit.Buffs?.Any(buff => buff.Paralyzed) ?? false;
        }
        
        public static bool IsStunned(this IBattleUnitObject unit)
        {
            return unit.Buffs?.Any(buff => buff.Stunned) ?? false;
        }
        
        public static bool CanAct(this IBattleUnitObject unit)
        {
            return !unit.IsParalyzed();
        }
        
        public static bool HasDecliningParalysis(this IBattleUnitObject unit)
        {
            return unit.Buffs?.Any(b => b.Paralyzed && b.DeclinesWhenTakesDamage) ?? false;
        }
        
        public static bool HasDecliningStun(this IBattleUnitObject unit)
        {
            return unit.Buffs?.Any(b => b.Stunned && b.DeclinesWhenTakesDamage) ?? false;
        }
        
        public static IEnumerable<IBuffObject> GetBuffsToRemoveOnDamage(this IBattleUnitObject unit)
        {
            if (unit.Buffs == null || unit.Buffs.Count == 0)
                return Enumerable.Empty<IBuffObject>();
                
            return unit.Buffs.Where(b => b.DeclinesWhenTakesDamage);
        }
        
        public static IReadOnlyList<IBuffObject> GetBuffsRemainingAfterDamage(this IBattleUnitObject unit)
        {
            if (unit.Buffs == null || unit.Buffs.Count == 0)
                return Array.Empty<IBuffObject>();
                
            return unit.Buffs.Where(b => !b.DeclinesWhenTakesDamage).ToList();
        }
        
        public static int GetEffectiveMaxHealth(this IBattleUnitObject unit)
        {
            var baseHealth = unit.GlobalUnit?.UnitType?.Health ?? 0;
            if (unit.Buffs == null || unit.Buffs.Count == 0)
                return baseHealth;

            var flatBonus = unit.Buffs.Sum(b => b.HealthBonus);
            var percentageBonus = unit.Buffs.Sum(b => b.HealthBonusPercentage);
            
            return baseHealth + flatBonus + (baseHealth * percentageBonus / 100);
        }
        
        public static int GetEffectiveAttack(this IBattleUnitObject unit)
        {
            var baseAttack = unit.GlobalUnit?.UnitType?.Attack ?? 0;
            var heroBonus = unit.HeroStats?.Attack ?? 0;
            if (unit.Buffs == null || unit.Buffs.Count == 0)
                return baseAttack + heroBonus;

            var flatBonus = unit.Buffs.Sum(b => b.AttackBonus);
            var percentageBonus = unit.Buffs.Sum(b => b.AttackBonusPercentage);
            
            return baseAttack + heroBonus + flatBonus + (baseAttack * percentageBonus / 100);
        }
        
        public static int GetEffectiveDefense(this IBattleUnitObject unit)
        {
            var baseDefense = unit.GlobalUnit?.UnitType?.Defense ?? 0;
            var heroBonus = unit.HeroStats?.Defense ?? 0;
            if (unit.Buffs == null || unit.Buffs.Count == 0)
                return baseDefense + heroBonus;

            var flatBonus = unit.Buffs.Sum(b => b.DefenseBonus);
            var percentageBonus = unit.Buffs.Sum(b => b.DefenseBonusPercentage);
            
            return baseDefense + heroBonus + flatBonus + (baseDefense * percentageBonus / 100);
        }
        
        public static int GetEffectiveMinDamage(this IBattleUnitObject unit, int baseMinDamage)
        {
            if (unit.Buffs == null || unit.Buffs.Count == 0)
                return Math.Max(0, baseMinDamage);

            var flatBonus = unit.Buffs.Sum(b => b.MinDamageBonus);
            var percentageBonus = unit.Buffs.Sum(b => b.MinDamageBonusPercentage);
            
            var result = baseMinDamage + flatBonus + (baseMinDamage * percentageBonus / 100);
            return Math.Max(0, result);
        }

        public static int GetEffectiveMaxDamage(this IBattleUnitObject unit, int baseMinDamage, int baseMaxDamage)
        {
            var effectiveMin = unit.GetEffectiveMinDamage(baseMinDamage);
            
            if (unit.Buffs == null || unit.Buffs.Count == 0)
                return Math.Max(effectiveMin, baseMaxDamage);

            var flatBonus = unit.Buffs.Sum(b => b.MaxDamageBonus);
            var percentageBonus = unit.Buffs.Sum(b => b.MaxDamageBonusPercentage);
            
            var result = baseMaxDamage + flatBonus + (baseMaxDamage * percentageBonus / 100);
            return Math.Max(effectiveMin, result);
        }
        
        public static int GetDamageReturnPercentage(this IBattleUnitObject unit, int attackRange)
        {
            if (unit.Buffs == null || unit.Buffs.Count == 0)
                return 0;

            var totalReturnPercentage = 0;
            
            foreach (var buff in unit.Buffs)
            {
                if (buff.DamageReturnPercentage <= 0)
                    continue;
                
                var maxRange = buff.DamageReturnMaxRange;
                if (maxRange > 0 && attackRange > maxRange)
                    continue;
                
                totalReturnPercentage += buff.DamageReturnPercentage;
            }
            
            return totalReturnPercentage;
        }
        
        public static (int Percentage, bool CanResurrect) GetVampireStats(this IBattleUnitObject unit)
        {
            if (unit.Buffs == null || unit.Buffs.Count == 0)
                return (0, false);

            var vampirePercentage = 0;
            var canResurrect = false;
            
            foreach (var buff in unit.Buffs)
            {
                if (buff.VampirePercentage > 0)
                {
                    vampirePercentage += buff.VampirePercentage;
                    if (buff.VampireCanResurrect)
                        canResurrect = true;
                }
            }
            
            return (vampirePercentage, canResurrect);
        }
        
        /// <summary>
        /// Gets effect type properties from the unit (buffs and, when available, other effects).
        /// </summary>
        public static IEnumerable<IEffectProperties> GetEffectTypeProperties(this IBattleUnitObject unit)
        {
            if (unit.Buffs == null || unit.Buffs.Count == 0)
                yield break;

            foreach (var buff in unit.Buffs)
                yield return buff;
        }

    }
}
