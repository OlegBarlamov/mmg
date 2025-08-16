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
        
        public override void Validate(CommandExecutionContext context, UnitAttackClientBattleMessage command)
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
        }

        public override async Task<ICmdExecutionResult> Execute(CommandExecutionContext context, UnitAttackClientBattleMessage command)
        {
            TargetActor.Column = command.MoveToCell.C;
            TargetActor.Row = command.MoveToCell.R;
            TargetActor.AttackFunctionsData[command.AttackIndex].BulletsCount -= 1;
            
            await context.BattleUnitsService.UpdateUnits(new[] { TargetActor });

            await context.MessageBroadcaster.BroadcastMessageAsync(
                new UnitMoveCommandFromServer(command.TurnIndex, command.Player, command.ActorId, command.MoveToCell)
            );

            await context.MessageBroadcaster.BroadcastMessageAsync(
                new UnitAttackCommandFromServer(command.TurnIndex, command.Player, command.ActorId,
                    command.TargetId, command.AttackIndex, false)
            );

            var unitTakesDamageData = UnitTakesDamageData.FromUnitAndTarget(
                TargetActor,
                _targetTarget,
                _attackFunction,
                _range,
                false,
                context.RandomProvider.Random);

            _targetTarget.GlobalUnit.Count = unitTakesDamageData.RemainingCount;
            _targetTarget.GlobalUnit.IsAlive = _targetTarget.GlobalUnit.Count > 0;

            await context.GlobalUnitsService.UpdateUnits(new[] { _targetTarget.GlobalUnit });

            _targetTarget.CurrentCount = unitTakesDamageData.RemainingCount;
            _targetTarget.CurrentHealth = unitTakesDamageData.RemainingHealth;

            await context.BattleUnitsService.UpdateUnits(new[] { _targetTarget });

            var serverUnitTakesDamage =
                new UnitTakesDamageCommandFromServer(command.TurnIndex, command.Player, command.TargetId)
                {
                    DamageTaken = unitTakesDamageData.DamageTaken,
                    KilledCount = unitTakesDamageData.KilledCount,
                    RemainingCount = unitTakesDamageData.RemainingCount,
                    RemainingHealth = unitTakesDamageData.RemainingHealth,
                };
            await context.MessageBroadcaster.BroadcastMessageAsync(serverUnitTakesDamage);

            if (_attackFunction.CanTargetCounterattack && _targetTarget.GlobalUnit.IsAlive)
            {
                var attackFunctionForCounterattack = FindAttackFunctionForCounterattack(_targetTarget, _range, context.BattleObject.Units);
                if (attackFunctionForCounterattack != null)
                {
                    await context.MessageBroadcaster.BroadcastMessageAsync(
                        new UnitAttackCommandFromServer(command.TurnIndex, command.Player, command.TargetId,
                            command.ActorId, attackFunctionForCounterattack.AttackIndex, true)
                    );

                    attackFunctionForCounterattack.CounterattacksUsed++;
                    attackFunctionForCounterattack.BulletsCount--;
                    await context.BattleUnitsService.UpdateUnits(new[] { _targetTarget });

                    unitTakesDamageData = UnitTakesDamageData.FromUnitAndTarget(
                        _targetTarget,
                        TargetActor,
                        _targetTarget.GlobalUnit.UnitType.Attacks[attackFunctionForCounterattack.AttackIndex],
                        _range,
                        true,
                        context.RandomProvider.Random);

                    TargetActor.GlobalUnit.Count = unitTakesDamageData.RemainingCount;
                    TargetActor.GlobalUnit.IsAlive = TargetActor.GlobalUnit.Count > 0;

                    await context.GlobalUnitsService.UpdateUnits(new[] { TargetActor.GlobalUnit });

                    TargetActor.CurrentCount = unitTakesDamageData.RemainingCount;
                    TargetActor.CurrentHealth = unitTakesDamageData.RemainingHealth;

                    await context.BattleUnitsService.UpdateUnits(new[] { TargetActor });

                    await context.MessageBroadcaster.BroadcastMessageAsync(
                        new UnitTakesDamageCommandFromServer(command.TurnIndex, command.Player, command.ActorId)
                        {
                            DamageTaken = unitTakesDamageData.DamageTaken,
                            KilledCount = unitTakesDamageData.KilledCount,
                            RemainingCount = unitTakesDamageData.RemainingCount,
                            RemainingHealth = unitTakesDamageData.RemainingHealth,
                        });
                }
            }

            return new CmdExecutionResult(true);
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