using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core;
using Epic.Core.ClientMessages;
using Epic.Core.Logic.Erros;
using Epic.Core.ServerMessages;
using Epic.Core.Services.Battles;
using Epic.Data.BattleUnits;
using Epic.Data.UnitTypes.Subtypes;
using Epic.Logic.Battle.Formulas;
using Epic.Logic.Battle.Map;

namespace Epic.Logic.Battle.Commands
{
    internal class UnitAttacksHandler : BaseTypedCommandHandler<UnitAttackClientBattleMessage>
    {
        private MutableBattleUnitObject _targetTarget;
        private IAttackFunctionType _attackFunction;
        private int _range;
        
        public override Task Validate(CommandExecutionContext context, UnitAttackClientBattleMessage command)
        {
            ValidateTargetActor(context, command.ActorId);
            ValidateExpectedTurn(context, command.TurnIndex, command.Player);
            
            //TODO Check if it is reachable
            
            _targetTarget = context.BattleObject.Units.FirstOrDefault(x => x.Id.ToString() == command.TargetId);
            if (_targetTarget == null)
                throw new BattleLogicException("Not found target unit for client command");

            var availableAttacks = TargetActor.GlobalUnit.UnitType.Attacks;
            if (command.AttackIndex < 0 || command.AttackIndex >= availableAttacks.Count)
                throw new BattleLogicException($"Wrong Attack Type index {command.AttackIndex}");
            _attackFunction = availableAttacks[command.AttackIndex];
            if (_attackFunction.StayOnly && command.MoveToCell != new HexoPoint(TargetActor.Column, TargetActor.Row))
                throw new BattleLogicException($"The target Attack Type {_attackFunction.Name} does not allow moving");
            _range = OddRHexoGrid.Distance(command.MoveToCell, _targetTarget);
            if (_range < _attackFunction.AttackMinRange || _range > _attackFunction.AttackMaxRange)
                throw new BattleLogicException("The target is out of range for attack");
            if (_attackFunction.EnemyInRangeDisablesAttack > 0 && MapUtils.IsEnemyInRange(TargetActor,
                    _attackFunction.EnemyInRangeDisablesAttack,
                    context.BattleObject.Units))
                throw new BattleLogicException(
                    $"The attack is blocked by an enemy in range {_attackFunction.EnemyInRangeDisablesAttack}");
            if (TargetActor.AttackFunctionsData[command.AttackIndex].BulletsCount < 1)
                throw new BattleLogicException(
                    $"The attack does not have enough bullets {_attackFunction.EnemyInRangeDisablesAttack}");
            
            return Task.CompletedTask;
        }

        public override async Task<ICmdExecutionResult> Execute(CommandExecutionContext context, UnitAttackClientBattleMessage command)
        {
            var originalColumn = TargetActor.Column;
            var originalRow = TargetActor.Row;
            TargetActor.Column = command.MoveToCell.C;
            TargetActor.Row = command.MoveToCell.R;
            
            await context.MessageBroadcaster.BroadcastMessageAsync(
                new UnitMoveCommandFromServer(command.TurnIndex, command.Player, command.ActorId, command.MoveToCell)
            );
            
            await ProcessAttack(
                context,
                TargetActor,
                _targetTarget,
                _attackFunction,
                command.AttackIndex,
                command,
                false,
                _range);

            if (_attackFunction.CanTargetCounterattack && _targetTarget.GlobalUnit.IsAlive)
            {
                var attackFunctionForCounterattack = FindAttackFunctionForCounterattack(_targetTarget, _range, context.BattleObject.Units);
                if (attackFunctionForCounterattack != null)
                {
                    await ProcessAttack(
                        context,
                        _targetTarget,
                        TargetActor,
                        _targetTarget.GlobalUnit.UnitType.Attacks[attackFunctionForCounterattack.AttackIndex],
                        attackFunctionForCounterattack.AttackIndex,
                        command,
                        true,
                        _range);
                }
            }

            for (int i = 1; i < _attackFunction.AttacksCount; i++)
            {
                if (!_targetTarget.GlobalUnit.IsAlive || !TargetActor.GlobalUnit.IsAlive)
                    break;
                
                await ProcessAttack(
                    context,
                    TargetActor,
                    _targetTarget,
                    _attackFunction,
                    command.AttackIndex,
                    command,
                    false,
                    _range);
            }

            if (TargetActor.GlobalUnit.IsAlive && _attackFunction.MovesBackAfterAttack)
            {
                TargetActor.Column = originalColumn;
                TargetActor.Row = originalRow;
                
                await context.BattleUnitsService.UpdateUnits(new[] { TargetActor });
                
                await context.MessageBroadcaster.BroadcastMessageAsync(
                    new UnitMoveCommandFromServer(command.TurnIndex, command.Player, command.ActorId, new HexoPoint(
                        TargetActor.Column, TargetActor.Row))
                );
            }

            return new CmdExecutionResult(true);
        }

        private static async Task ProcessAttack(
            CommandExecutionContext context,
            MutableBattleUnitObject attacker,
            MutableBattleUnitObject target,
            IAttackFunctionType attackType,
            int attackIndex,
            UnitAttackClientBattleMessage command,
            bool isCounterAttack,
            int range
            )
        {
            await context.MessageBroadcaster.BroadcastMessageAsync(
                new UnitAttackCommandFromServer(command.TurnIndex, command.Player, attacker.Id.ToString(),
                    target.Id.ToString(), attackIndex, isCounterAttack)
            );
            
            attacker.AttackFunctionsData[attackIndex].BulletsCount -= 1;
            if (isCounterAttack)
                attacker.AttackFunctionsData[attackIndex].CounterattacksUsed += 1;
            
            await context.BattleUnitsService.UpdateUnits(new[] { attacker });

            // Process damage to primary target
            var unitTakesDamageData = UnitTakesDamageData.FromUnitAndTarget(
                attacker,
                target,
                attackType,
                range,
                isCounterAttack,
                context.RandomProvider);

            target.GlobalUnit.Count = unitTakesDamageData.RemainingCount;
            target.GlobalUnit.IsAlive = target.GlobalUnit.Count > 0;

            await context.GlobalUnitsService.UpdateUnits(new[] { target.GlobalUnit });

            target.CurrentCount = unitTakesDamageData.RemainingCount;
            target.CurrentHealth = unitTakesDamageData.RemainingHealth;

            await context.BattleUnitsService.UpdateUnits(new[] { target });

            var serverUnitTakesDamage =
                new UnitTakesDamageCommandFromServer(command.TurnIndex, command.Player, target.Id.ToString())
                {
                    DamageTaken = unitTakesDamageData.DamageTaken,
                    KilledCount = unitTakesDamageData.KilledCount,
                    RemainingCount = unitTakesDamageData.RemainingCount,
                    RemainingHealth = unitTakesDamageData.RemainingHealth,
                };
            await context.MessageBroadcaster.BroadcastMessageAsync(serverUnitTakesDamage);

            // Process PierceThrough - damage units behind target in straight line
            if (attackType.PierceThrough > 0)
            {
                var unitsBehindTarget = MapUtils.GetUnitsInLineBehindTarget(
                    attacker,
                    target,
                    attackType.PierceThrough,
                    context.BattleObject.Units,
                    context.BattleObject.Height,
                    context.BattleObject.Width);

                foreach (MutableBattleUnitObject unitBehind in unitsBehindTarget)
                {
                    // Apply same damage calculation to units behind
                    var behindDamageData = UnitTakesDamageData.FromUnitAndTarget(
                        attacker,
                        unitBehind,
                        attackType,
                        OddRHexoGrid.Distance(attacker, unitBehind),
                        isCounterAttack,
                        context.RandomProvider);

                    unitBehind.GlobalUnit.Count = behindDamageData.RemainingCount;
                    unitBehind.GlobalUnit.IsAlive = unitBehind.GlobalUnit.Count > 0;

                    await context.GlobalUnitsService.UpdateUnits(new[] { unitBehind.GlobalUnit });

                    unitBehind.CurrentCount = behindDamageData.RemainingCount;
                    unitBehind.CurrentHealth = behindDamageData.RemainingHealth;

                    await context.BattleUnitsService.UpdateUnits(new[] { unitBehind });

                    var behindUnitTakesDamage =
                        new UnitTakesDamageCommandFromServer(command.TurnIndex, command.Player, unitBehind.Id.ToString())
                        {
                            DamageTaken = behindDamageData.DamageTaken,
                            KilledCount = behindDamageData.KilledCount,
                            RemainingCount = behindDamageData.RemainingCount,
                            RemainingHealth = behindDamageData.RemainingHealth,
                        };
                    await context.MessageBroadcaster.BroadcastMessageAsync(behindUnitTakesDamage);
                }
            }

            // Process Splash - damage units around target (only for melee attacks)
            if (attackType.Splash > 0 && attackType.AttackMaxRange == 1)
            {
                var attackerPosition = new HexoPoint(attacker.Column, attacker.Row);
                var targetPosition = new HexoPoint(target.Column, target.Row);
                var splashPositions = MapUtils.GetSplashNeighbors(
                    attackerPosition,
                    targetPosition,
                    attackType.Splash,
                    context.BattleObject.Height,
                    context.BattleObject.Width);

                // Get units at splash positions (excluding the primary target and friendly units)
                var splashUnits = MapUtils.GetUnitsAtPositions(splashPositions, context.BattleObject.Units)
                    .Where(u => u.Id != target.Id && u.PlayerIndex != attacker.PlayerIndex)
                    .ToList();

                foreach (MutableBattleUnitObject splashUnit in splashUnits)
                {
                    // Apply same damage calculation to splash units
                    var splashDamageData = UnitTakesDamageData.FromUnitAndTarget(
                        attacker,
                        splashUnit,
                        attackType,
                        OddRHexoGrid.Distance(attacker, splashUnit),
                        isCounterAttack,
                        context.RandomProvider);

                    splashUnit.GlobalUnit.Count = splashDamageData.RemainingCount;
                    splashUnit.GlobalUnit.IsAlive = splashUnit.GlobalUnit.Count > 0;

                    await context.GlobalUnitsService.UpdateUnits(new[] { splashUnit.GlobalUnit });

                    splashUnit.CurrentCount = splashDamageData.RemainingCount;
                    splashUnit.CurrentHealth = splashDamageData.RemainingHealth;

                    await context.BattleUnitsService.UpdateUnits(new[] { splashUnit });

                    var splashUnitTakesDamage =
                        new UnitTakesDamageCommandFromServer(command.TurnIndex, command.Player, splashUnit.Id.ToString())
                        {
                            DamageTaken = splashDamageData.DamageTaken,
                            KilledCount = splashDamageData.KilledCount,
                            RemainingCount = splashDamageData.RemainingCount,
                            RemainingHealth = splashDamageData.RemainingHealth,
                        };
                    await context.MessageBroadcaster.BroadcastMessageAsync(splashUnitTakesDamage);
                }
            }
        }

        private static AttackFunctionStateEntity FindAttackFunctionForCounterattack(IBattleUnitObject unit, int range, IReadOnlyCollection<IBattleUnitObject> battleUnits)
        {
            var attackFunctionForCounterattack = unit.AttackFunctionsData.FirstOrDefault(x =>
            {
                var counterattackFunctionType = unit.GlobalUnit.UnitType.Attacks[x.AttackIndex];
                var enoughAttacks = counterattackFunctionType.CounterattacksCount - x.CounterattacksUsed > 0;
                return enoughAttacks && x.BulletsCount > 0 &&
                       range >= counterattackFunctionType.AttackMinRange &&
                       range <= counterattackFunctionType.AttackMaxRange &&
                       (counterattackFunctionType.EnemyInRangeDisablesAttack <= 0 ||
                        !MapUtils.IsEnemyInRange(unit,
                            counterattackFunctionType.EnemyInRangeDisablesAttack,
                            battleUnits));
            });
            return attackFunctionForCounterattack;
        }
    }
}