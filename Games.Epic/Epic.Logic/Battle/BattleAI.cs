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
            for (var index = 0; index < possibleAttacksWithTargets.Count; index++)
            {
                var attacksWithTarget = possibleAttacksWithTargets[index];
                var potentialDamage = UnitTakesDamageData.FromUnitAndTarget(
                    unit, 
                    attacksWithTarget.Item1, 
                    attacksWithTarget.Item2, 
                    OddRHexoGrid.Distance(unit, attacksWithTarget.Item1),
                    false,
                    _maxRandomProvider);

                if (potentialDamage.KilledCount > maxKilled ||
                    (potentialDamage.KilledCount == maxKilled && potentialDamage.DamageTaken > maxDamage))
                {
                    maxKilled = potentialDamage.KilledCount;
                    maxDamage = potentialDamage.DamageTaken;
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