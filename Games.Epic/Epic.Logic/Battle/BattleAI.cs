using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core;
using Epic.Core.ClientMessages;
using Epic.Core.Logic;
using Epic.Core.Services.Battles;
using Epic.Core.Services.Connection;
using Epic.Data.UnitTypes.Subtypes;
using Epic.Logic.Battle.Formulas;
using Epic.Logic.Battle.Map;
using FrameworkSDK.Common;
using JetBrains.Annotations;

namespace Epic.Logic.Battle
{
    internal class BattleAI
    {
        public IBattleObject Battle { get; }
        public IBattleLogic BattleLogic { get; }

        private readonly IBattleClientConnection _fakeConnection;
        private readonly IRandomService _maxRandomProvider = new MaxValueRandomProvider();

        public BattleAI([NotNull] IBattleObject battle, [NotNull] IBattleLogic battleLogic)
        {
            Battle = battle ?? throw new ArgumentNullException(nameof(battle));
            BattleLogic = battleLogic ?? throw new ArgumentNullException(nameof(battleLogic));

            _fakeConnection = new FakeClientConnection(Battle);
        }

        public Task ProcessAutoSkip(IBattleUnitObject unit)
        {
            return BattleLogic.OnClientMessage(_fakeConnection, new UnitPassClientBattleMessage
            {
                Player = unit.PlayerIndex.ToInBattlePlayerNumber(),
                TurnIndex = Battle.TurnNumber,
                ActorId = unit.Id.ToString(),
                CommandId = Guid.NewGuid().ToString(),
            });
        }

        public async Task ProcessAction(IBattleUnitObject unit)
        {
            var enemies = Battle.Units.Where(x => x.PlayerIndex != unit.PlayerIndex && x.GlobalUnit.IsAlive).ToList();
            var attacks = unit.GlobalUnit.UnitType.Attacks;

            Tuple<IBattleUnitObject, int> closestTarget = null;
            var cellsToMove = MapUtils.GetCellsForUnitMove(Battle, Battle.Obstacles, unit, unit.GlobalUnit.UnitType.Movement);
            var possibleAttacksWithTargets = new List<Tuple<IBattleUnitObject, IAttackFunctionType, HexoPoint, int>>();
            
            foreach (var enemy in enemies)
            {
                var distance = OddRHexoGrid.Distance(unit, enemy);
                if (closestTarget == null || distance < closestTarget.Item2)
                    closestTarget = new Tuple<IBattleUnitObject, int>(enemy, distance);
                
                for (int i = 0; i < attacks.Count; i++)
                {
                    var attack = attacks[i];

                    var attackData = unit.AttackFunctionsData.First(x => x.AttackIndex == i);
                    if (attackData.BulletsCount < 1)
                        continue;
                
                    if (attack.EnemyInRangeDisablesAttack > 0 &&
                        enemies.Any(x => MapUtils.IsEnemyInRange(unit, attack.EnemyInRangeDisablesAttack, enemies))) 
                        continue;

                    if (distance >= attack.AttackMinRange && distance <= attack.AttackMaxRange)
                    {
                        possibleAttacksWithTargets.Add(new Tuple<IBattleUnitObject, IAttackFunctionType, HexoPoint, int>(enemy, attack,
                            new HexoPoint(unit.Column, unit.Row), i));
                        continue;
                    }

                    if (attack.StayOnly)
                        continue;

                    foreach (var point in cellsToMove)
                    {
                        distance = OddRHexoGrid.Distance(point, enemy);
                        if (distance < attack.AttackMaxRange || distance > attack.AttackMaxRange)
                            continue;
                        
                        possibleAttacksWithTargets.Add(new Tuple<IBattleUnitObject, IAttackFunctionType, HexoPoint, int>(enemy, attack, point, i));
                    }
                }
            }

            
            int maxAttackIndex = -1;
            var maxDamage = 0;
            var maxKilled = 0;
            var maxValue = 0; // Combined value considering cumulative damage and friendly fire penalty
            // Iterate in reverse order so that when damage/kills are equal, attacks that appear earlier 
            // in the list (earlier attack types) are checked later and overwrite the selection, 
            // giving priority to earlier attack types in the unit's attack list.
            for (var index = possibleAttacksWithTargets.Count - 1; index >= 0; index--)
            {
                var attacksWithTarget = possibleAttacksWithTargets[index];
                var targetEnemy = attacksWithTarget.Item1;
                var attackType = attacksWithTarget.Item2;
                var attackerPosition = attacksWithTarget.Item3;
                
                // Calculate all targets and their damages using the shared calculator
                var allTargets = AttackTargetsCalculator.CalculateAllTargets<IBattleUnitObject>(
                    unit,
                    attackerPosition,
                    targetEnemy,
                    attackType,
                    false,
                    Battle.Units,
                    Battle.Height,
                    Battle.Width,
                    _maxRandomProvider);

                // Calculate cumulative damage and friendly fire penalty
                var cumulativeDamage = 0;
                var cumulativeKilled = 0;
                var friendlyFirePenalty = 0;
                var paralyzedWithDeclineBuffPenalty = 0;

                foreach (var targetDamage in allTargets)
                {
                    if (targetDamage.Target.PlayerIndex == unit.PlayerIndex)
                    {
                        // Friendly unit hit - apply penalty
                        friendlyFirePenalty += targetDamage.DamageData.DamageTaken;
                    }
                    else
                    {
                        // Enemy unit hit - add to cumulative damage
                        cumulativeDamage += targetDamage.DamageData.DamageTaken;
                        cumulativeKilled += targetDamage.DamageData.KilledCount;
                        
                        // Check if this enemy is paralyzed and has buffs that decline when taking damage
                        // Attacking such targets would break their paralysis - apply penalty
                        if (targetDamage.DamageData.DamageTaken > 0 && 
                            targetDamage.Target.Buffs != null &&
                            targetDamage.Target.Buffs.Any(b => 
                                b.BuffType?.Paralyzed == true && 
                                b.BuffType?.DeclinesWhenTakesDamage == true))
                        {
                            // Large penalty to deprioritize breaking paralysis
                            paralyzedWithDeclineBuffPenalty += 10000;
                        }
                    }
                }

                // Calculate total value: cumulative damage minus penalties
                // Paralyzed units with DeclinesWhenTakesDamage buffs are deprioritized
                var totalValue = cumulativeDamage - friendlyFirePenalty - paralyzedWithDeclineBuffPenalty;
                
                // Prioritize kills, then total value (damage - friendly fire)
                if (cumulativeKilled > maxKilled ||
                    (cumulativeKilled == maxKilled && totalValue > maxValue))
                {
                    maxKilled = cumulativeKilled;
                    maxDamage = cumulativeDamage;
                    maxValue = totalValue;
                    maxAttackIndex = index;
                } 
            }

            if (maxAttackIndex > -1)
            {
                var maxAttackObj = possibleAttacksWithTargets[maxAttackIndex];

                var moveToPoint = maxAttackObj.Item3;
                
                await BattleLogic.OnClientMessage(_fakeConnection, new UnitAttackClientBattleMessage
                {
                    Player = unit.PlayerIndex.ToInBattlePlayerNumber(),
                    TurnIndex = Battle.TurnNumber,
                    ActorId = unit.Id.ToString(),
                    MoveToCell = moveToPoint,
                    AttackIndex = maxAttackObj.Item4,
                    CommandId = Guid.NewGuid().ToString(),
                    TargetId = maxAttackObj.Item1.Id.ToString(),
                });
                
                return;
            }

            if (!unit.Waited)
            {
                await BattleLogic.OnClientMessage(_fakeConnection, new UnitWaitClientBattleMessage
                {
                    Player = unit.PlayerIndex.ToInBattlePlayerNumber(),
                    TurnIndex = Battle.TurnNumber,
                    ActorId = unit.Id.ToString(),
                    CommandId = Guid.NewGuid().ToString(),
                });
                return;
            }

            if (closestTarget != null)
            {
                var distanceToClosestTarget = closestTarget.Item2;
                var finalMove = new HexoPoint(unit.Column, unit.Row);
                foreach (var point in cellsToMove)
                {
                    var d = OddRHexoGrid.Distance(point, closestTarget.Item1);
                    if (d < distanceToClosestTarget)
                    {
                        distanceToClosestTarget = d;
                        finalMove = point;
                    }
                }

                // If no direct path found that gets closer, try to find best move toward target
                // This helps navigate around large obstacles
                if (finalMove.C == unit.Column && finalMove.R == unit.Row && cellsToMove.Count > 0)
                {
                    var targetPoint = new HexoPoint(closestTarget.Item1.Column, closestTarget.Item1.Row);
                    var bestMove = MapUtils.FindBestMoveTowardTarget(
                        Battle, 
                        Battle.Obstacles, 
                        unit, 
                        targetPoint, 
                        unit.GlobalUnit.UnitType.Movement, 
                        cellsToMove);
                    
                    if (bestMove.HasValue)
                    {
                        finalMove = bestMove.Value;
                    }
                }

                if (finalMove.C != unit.Column || finalMove.R != unit.Row)
                {
                    await BattleLogic.OnClientMessage(_fakeConnection, new UnitMoveClientBattleMessage
                    {
                        Player = unit.PlayerIndex.ToInBattlePlayerNumber(),
                        TurnIndex = Battle.TurnNumber,
                        ActorId = unit.Id.ToString(),
                        MoveToCell = finalMove,
                        CommandId = Guid.NewGuid().ToString(),
                    });
                    return;
                }
            }

            await BattleLogic.OnClientMessage(_fakeConnection, new UnitPassClientBattleMessage
            {
                Player = unit.PlayerIndex.ToInBattlePlayerNumber(),
                TurnIndex = Battle.TurnNumber,
                ActorId = unit.Id.ToString(),
                CommandId = Guid.NewGuid().ToString(),
            });
        }
    }
}