using System;
using System.Collections.Generic;
using System.Linq;
using Epic.Core.Services.Battles;
using Epic.Core.Services.Buffs;

namespace Epic.Logic.Battle
{
    public static class BattleUnitBuffExtensions
    {
        public static bool IsParalyzed(this IBattleUnitObject unit)
        {
            return unit.Buffs?.Any(buff => buff.BuffType?.Paralyzed == true) ?? false;
        }
        
        public static bool IsStunned(this IBattleUnitObject unit)
        {
            return unit.Buffs?.Any(buff => buff.BuffType?.Stunned == true) ?? false;
        }
        
        public static bool CanAct(this IBattleUnitObject unit)
        {
            return !unit.IsParalyzed();
        }
        
        public static bool HasDecliningParalysis(this IBattleUnitObject unit)
        {
            return unit.Buffs?.Any(b => 
                b.BuffType?.Paralyzed == true && 
                b.BuffType?.DeclinesWhenTakesDamage == true) ?? false;
        }
        
        public static bool HasDecliningStun(this IBattleUnitObject unit)
        {
            return unit.Buffs?.Any(b => 
                b.BuffType?.Stunned == true && 
                b.BuffType?.DeclinesWhenTakesDamage == true) ?? false;
        }
        
        public static IEnumerable<IBuffObject> GetBuffsToRemoveOnDamage(this IBattleUnitObject unit)
        {
            if (unit.Buffs == null || unit.Buffs.Count == 0)
                return Enumerable.Empty<IBuffObject>();
                
            return unit.Buffs.Where(b => b.BuffType?.DeclinesWhenTakesDamage == true);
        }
        
        public static IReadOnlyList<IBuffObject> GetBuffsRemainingAfterDamage(this IBattleUnitObject unit)
        {
            if (unit.Buffs == null || unit.Buffs.Count == 0)
                return Array.Empty<IBuffObject>();
                
            return unit.Buffs.Where(b => b.BuffType?.DeclinesWhenTakesDamage != true).ToList();
        }
        
        public static int GetEffectiveMaxHealth(this IBattleUnitObject unit)
        {
            var baseHealth = unit.GlobalUnit?.UnitType?.Health ?? 0;
            if (unit.Buffs == null || unit.Buffs.Count == 0)
                return baseHealth;

            var flatBonus = unit.Buffs.Where(b => b.BuffType != null).Sum(b => b.BuffType.HealthBonus);
            var percentageBonus = unit.Buffs.Where(b => b.BuffType != null).Sum(b => b.BuffType.HealthBonusPercentage);
            
            return baseHealth + flatBonus + (baseHealth * percentageBonus / 100);
        }
        
        public static int GetEffectiveAttack(this IBattleUnitObject unit)
        {
            var baseAttack = unit.GlobalUnit?.UnitType?.Attack ?? 0;
            var heroBonus = unit.HeroStats?.Attack ?? 0;
            if (unit.Buffs == null || unit.Buffs.Count == 0)
                return baseAttack + heroBonus;

            var flatBonus = unit.Buffs.Where(b => b.BuffType != null).Sum(b => b.BuffType.AttackBonus);
            var percentageBonus = unit.Buffs.Where(b => b.BuffType != null).Sum(b => b.BuffType.AttackBonusPercentage);
            
            return baseAttack + heroBonus + flatBonus + (baseAttack * percentageBonus / 100);
        }
        
        public static int GetEffectiveDefense(this IBattleUnitObject unit)
        {
            var baseDefense = unit.GlobalUnit?.UnitType?.Defense ?? 0;
            var heroBonus = unit.HeroStats?.Defense ?? 0;
            if (unit.Buffs == null || unit.Buffs.Count == 0)
                return baseDefense + heroBonus;

            var flatBonus = unit.Buffs.Where(b => b.BuffType != null).Sum(b => b.BuffType.DefenseBonus);
            var percentageBonus = unit.Buffs.Where(b => b.BuffType != null).Sum(b => b.BuffType.DefenseBonusPercentage);
            
            return baseDefense + heroBonus + flatBonus + (baseDefense * percentageBonus / 100);
        }
        
        public static int GetEffectiveMinDamage(this IBattleUnitObject unit, int baseMinDamage)
        {
            if (unit.Buffs == null || unit.Buffs.Count == 0)
                return Math.Max(0, baseMinDamage);

            var flatBonus = unit.Buffs.Where(b => b.BuffType != null).Sum(b => b.BuffType.MinDamageBonus);
            var percentageBonus = unit.Buffs.Where(b => b.BuffType != null).Sum(b => b.BuffType.MinDamageBonusPercentage);
            
            var result = baseMinDamage + flatBonus + (baseMinDamage * percentageBonus / 100);
            return Math.Max(0, result);
        }

        public static int GetEffectiveMaxDamage(this IBattleUnitObject unit, int baseMinDamage, int baseMaxDamage)
        {
            var effectiveMin = unit.GetEffectiveMinDamage(baseMinDamage);
            
            if (unit.Buffs == null || unit.Buffs.Count == 0)
                return Math.Max(effectiveMin, baseMaxDamage);

            var flatBonus = unit.Buffs.Where(b => b.BuffType != null).Sum(b => b.BuffType.MaxDamageBonus);
            var percentageBonus = unit.Buffs.Where(b => b.BuffType != null).Sum(b => b.BuffType.MaxDamageBonusPercentage);
            
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
                if (buff.BuffType == null || buff.BuffType.DamageReturnPercentage <= 0)
                    continue;
                
                var maxRange = buff.BuffType.DamageReturnMaxRange;
                if (maxRange > 0 && attackRange > maxRange)
                    continue;
                
                totalReturnPercentage += buff.BuffType.DamageReturnPercentage;
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
                if (buff.BuffType != null && buff.BuffType.VampirePercentage > 0)
                {
                    vampirePercentage += buff.BuffType.VampirePercentage;
                    if (buff.BuffType.VampireCanResurrect)
                        canResurrect = true;
                }
            }
            
            return (vampirePercentage, canResurrect);
        }
        
        public static (int Flat, int Percentage, bool CanResurrect) GetHealingStats(this IBattleUnitObject unit)
        {
            if (unit.Buffs == null || unit.Buffs.Count == 0)
                return (0, 0, false);

            var totalFlatHeal = 0;
            var totalPercentageHeal = 0;
            var canResurrect = false;

            foreach (var buff in unit.Buffs)
            {
                if (buff.BuffType == null)
                    continue;

                totalFlatHeal += buff.BuffType.Heals;
                totalPercentageHeal += buff.BuffType.HealsPercentage;
                if (buff.BuffType.HealCanResurrect)
                    canResurrect = true;
            }

            return (totalFlatHeal, totalPercentageHeal, canResurrect);
        }
        
        public static int CalculateDamageOverTime(this IBattleUnitObject unit, Func<int, int, int> randomInRange)
        {
            if (unit.Buffs == null || unit.Buffs.Count == 0)
                return 0;

            var totalDamage = 0;

            foreach (var buff in unit.Buffs)
            {
                if (buff.BuffType == null)
                    continue;
                
                var minDmg = buff.BuffType.TakesDamageMin;
                var maxDmg = buff.BuffType.TakesDamageMax;
                if (minDmg > 0 || maxDmg > 0)
                {
                    if (maxDmg < minDmg) maxDmg = minDmg;
                    totalDamage += randomInRange(minDmg, maxDmg + 1);
                }
            }

            return totalDamage;
        }
    }
}
