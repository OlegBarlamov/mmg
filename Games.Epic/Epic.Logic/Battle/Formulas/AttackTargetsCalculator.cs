using System.Collections.Generic;
using System.Linq;
using Epic.Core;
using Epic.Core.Services.Battles;
using Epic.Data.UnitTypes.Subtypes;
using Epic.Logic.Battle.Map;
using FrameworkSDK.Common;

namespace Epic.Logic.Battle.Formulas
{
    public class AttackTargetDamage<TBattleUnit> where TBattleUnit : IBattleUnitObject
    {
        public TBattleUnit Target { get; set; }
        public UnitTakesDamageData DamageData { get; set; }
    }
    
    public static class AttackTargetsCalculator
    {
        public static List<AttackTargetDamage<TBattleUnit>> CalculateAllTargets<TBattleUnit>(
            IBattleUnitObject attacker,
            HexoPoint attackerPosition,
            TBattleUnit primaryTarget,
            IAttackFunctionType attackType,
            bool isCounterAttack,
            IReadOnlyCollection<TBattleUnit> allUnits,
            int mapHeight,
            int mapWidth,
            IRandomService randomProvider) where TBattleUnit : IBattleUnitObject
        {
            var result = new List<AttackTargetDamage<TBattleUnit>>();
            var primaryTargetPosition = new HexoPoint(primaryTarget.Column, primaryTarget.Row);
            var primaryDistance = OddRHexoGrid.Distance(attackerPosition, primaryTargetPosition);

            // Primary target damage
            var primaryDamage = UnitTakesDamageData.FromUnitAndTarget(
                attacker,
                primaryTarget,
                attackType,
                primaryDistance,
                isCounterAttack,
                randomProvider);

            result.Add(new AttackTargetDamage<TBattleUnit>
            {
                Target = primaryTarget,
                DamageData = primaryDamage
            });

            // PierceThrough targets - units behind the primary target
            if (attackType.PierceThrough > 0)
            {
                var unitsBehind = MapUtils.GetUnitsInLineBehindTarget(
                    attackerPosition,
                    primaryTarget,
                    attackType.PierceThrough,
                    allUnits,
                    mapHeight,
                    mapWidth);

                foreach (var unitBehind in unitsBehind)
                {
                    var behindDistance = OddRHexoGrid.Distance(attackerPosition, unitBehind);
                    var behindDamage = UnitTakesDamageData.FromUnitAndTarget(
                        attacker,
                        unitBehind,
                        attackType,
                        behindDistance,
                        isCounterAttack,
                        randomProvider);

                    result.Add(new AttackTargetDamage<TBattleUnit>
                    {
                        Target = unitBehind,
                        DamageData = behindDamage
                    });
                }
            }

            // Splash targets.
            // - For melee (max range == 1) we keep existing "cleave around attacker" semantics.
            // - For ranged (max range > 1) splash is treated as a radius around the primary target.
            if (attackType.Splash > 0)
            {
                List<HexoPoint> splashPositions;
                if (attackType.AttackMaxRange == 1)
                {
                    splashPositions = MapUtils.GetSplashNeighbors(
                        attackerPosition,
                        primaryTargetPosition,
                        attackType.Splash,
                        mapHeight,
                        mapWidth);
                }
                else
                {
                    splashPositions = MapUtils.GetCellsWithinRadius(
                        primaryTargetPosition,
                        attackType.Splash,
                        mapHeight,
                        mapWidth,
                        includeCenter: false);
                }

                bool includeFriendlyUnits = attackType.AttackMaxRange > 1; 
                var splashUnits = MapUtils.GetUnitsAtPositions(splashPositions, allUnits)
                    .Where(u => u.Id != primaryTarget.Id && (includeFriendlyUnits || u.PlayerIndex != attacker.PlayerIndex))
                    .ToList();

                foreach (var splashUnit in splashUnits)
                {
                    var splashDistance = OddRHexoGrid.Distance(attackerPosition, splashUnit);
                    var splashDamage = UnitTakesDamageData.FromUnitAndTarget(
                        attacker,
                        splashUnit,
                        attackType,
                        splashDistance,
                        isCounterAttack,
                        randomProvider);

                    result.Add(new AttackTargetDamage<TBattleUnit>
                    {
                        Target = splashUnit,
                        DamageData = splashDamage
                    });
                }
            }

            return result;
        }
    }
}
