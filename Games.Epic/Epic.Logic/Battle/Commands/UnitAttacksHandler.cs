using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core;
using Epic.Core.ClientMessages;
using Epic.Core.Logic.Erros;
using Epic.Core.ServerMessages;
using Epic.Core.Services.Battles;
using Epic.Core.Services.Buffs;
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
            
            // Stunned units cannot move as part of attack
            var isStunned = TargetActor.Buffs?.Any(b => b.BuffType?.Stunned == true) ?? false;
            if (isStunned && command.MoveToCell != new HexoPoint(TargetActor.Column, TargetActor.Row))
                throw new BattleLogicException("Stunned units cannot move");
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

            // Paralyzed units cannot counterattack
            var isTargetParalyzed = _targetTarget.Buffs?.Any(b => b.BuffType?.Paralyzed == true) ?? false;
            
            if (_attackFunction.CanTargetCounterattack && _targetTarget.GlobalUnit.IsAlive && !isTargetParalyzed)
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

            // Calculate all targets and their damages using the shared calculator
            var attackerPosition = new HexoPoint(attacker.Column, attacker.Row);
            var allTargets = AttackTargetsCalculator.CalculateAllTargets<MutableBattleUnitObject>(
                attacker,
                attackerPosition,
                target,
                attackType,
                isCounterAttack,
                context.BattleObject.Units,
                context.BattleObject.Height,
                context.BattleObject.Width,
                context.RandomProvider);

            // Track total damage dealt to enemies for vampire healing
            var totalEnemyDamage = 0;

            // Apply damage to all affected targets
            foreach (var targetDamage in allTargets)
            {
                var mutableTarget = targetDamage.Target;

                mutableTarget.GlobalUnit.Count = targetDamage.DamageData.RemainingCount;
                mutableTarget.GlobalUnit.IsAlive = mutableTarget.GlobalUnit.Count > 0;

                await context.GlobalUnitsService.UpdateUnits(new[] { mutableTarget.GlobalUnit });

                mutableTarget.CurrentCount = targetDamage.DamageData.RemainingCount;
                mutableTarget.CurrentHealth = targetDamage.DamageData.RemainingHealth;

                await context.BattleUnitsService.UpdateUnits(new[] { mutableTarget });

                var serverUnitTakesDamage =
                    new UnitTakesDamageCommandFromServer(command.TurnIndex, command.Player, mutableTarget.Id.ToString())
                    {
                        DamageTaken = targetDamage.DamageData.DamageTaken,
                        KilledCount = targetDamage.DamageData.KilledCount,
                        RemainingCount = targetDamage.DamageData.RemainingCount,
                        RemainingHealth = targetDamage.DamageData.RemainingHealth,
                    };
                await context.MessageBroadcaster.BroadcastMessageAsync(serverUnitTakesDamage);
                
                // Track enemy damage for vampire healing (exclude friendly fire)
                if (mutableTarget.PlayerIndex != attacker.PlayerIndex)
                {
                    totalEnemyDamage += targetDamage.DamageData.DamageTaken;
                }
                
                // Remove buffs that decline when taking damage
                if (targetDamage.DamageData.DamageTaken > 0)
                {
                    await RemoveBuffsThatDeclineOnDamage(context, mutableTarget, command);
                    
                    // Apply damage return from target's buffs back to attacker
                    if (attacker.GlobalUnit.IsAlive)
                    {
                        await ApplyDamageReturn(context, mutableTarget, attacker, targetDamage.DamageData.DamageTaken, range, command);
                    }
                }
                
                if (mutableTarget.GlobalUnit.IsAlive)
                {
                    await ApplyAttackBuffsToTarget(context, mutableTarget, attackType, command);
                }
            }
            
            // Apply vampire healing to the attacker if they have vampire buffs
            if (totalEnemyDamage > 0 && attacker.GlobalUnit.IsAlive)
            {
                await ApplyVampireHealing(context, attacker, totalEnemyDamage, command);
            }
        }

        private static async Task ApplyDamageReturn(
            CommandExecutionContext context,
            MutableBattleUnitObject target,
            MutableBattleUnitObject attacker,
            int damageTaken,
            int range,
            UnitAttackClientBattleMessage command)
        {
            if (target.Buffs == null || target.Buffs.Count == 0)
                return;

            // Calculate total damage return percentage from all buffs that are in range
            var totalReturnPercentage = 0;
            
            foreach (var buff in target.Buffs)
            {
                if (buff.BuffType == null || buff.BuffType.DamageReturnPercentage <= 0)
                    continue;
                
                // Check if attacker is within the buff's max range (0 = any range)
                var maxRange = buff.BuffType.DamageReturnMaxRange;
                if (maxRange > 0 && range > maxRange)
                    continue;
                
                totalReturnPercentage += buff.BuffType.DamageReturnPercentage;
            }
            
            if (totalReturnPercentage <= 0)
                return;
            
            // Calculate damage to return
            var damageToReturn = damageTaken * totalReturnPercentage / 100;
            if (damageToReturn <= 0)
                return;
            
            var unitHealth = attacker.GlobalUnit.UnitType.Health;
            var currentCount = attacker.CurrentCount;
            var currentHealth = attacker.CurrentHealth;
            
            // Calculate damage application
            var totalCurrentHp = (currentCount - 1) * unitHealth + currentHealth;
            var totalNewHp = totalCurrentHp - damageToReturn;
            
            if (totalNewHp < 0)
                totalNewHp = 0;
            
            // Calculate new count and health
            int newCount;
            int newHealth;
            int killedCount;
            
            if (totalNewHp <= 0)
            {
                newCount = 0;
                newHealth = 0;
                killedCount = currentCount;
            }
            else
            {
                newCount = (totalNewHp + unitHealth - 1) / unitHealth; // Ceiling division
                newHealth = totalNewHp - (newCount - 1) * unitHealth;
                killedCount = currentCount - newCount;
            }
            
            var actualDamageTaken = totalCurrentHp - totalNewHp;
            
            if (actualDamageTaken <= 0)
                return;
            
            // Update the attacker's state
            attacker.CurrentCount = newCount;
            attacker.CurrentHealth = newHealth;
            attacker.GlobalUnit.Count = newCount;
            attacker.GlobalUnit.IsAlive = newCount > 0;
            
            await context.GlobalUnitsService.UpdateUnits(new[] { attacker.GlobalUnit });
            await context.BattleUnitsService.UpdateUnits(new[] { attacker });
            
            // Broadcast damage message
            var damageMessage = new UnitTakesDamageCommandFromServer(
                command.TurnIndex,
                command.Player,
                attacker.Id.ToString())
            {
                DamageTaken = actualDamageTaken,
                KilledCount = killedCount,
                RemainingCount = newCount,
                RemainingHealth = newHealth
            };
            await context.MessageBroadcaster.BroadcastMessageAsync(damageMessage);
        }

        private static async Task RemoveBuffsThatDeclineOnDamage(
            CommandExecutionContext context,
            MutableBattleUnitObject target,
            UnitAttackClientBattleMessage command)
        {
            if (target.Buffs == null || target.Buffs.Count == 0)
                return;

            var buffsToRemove = target.Buffs
                .Where(b => b.BuffType?.DeclinesWhenTakesDamage == true)
                .ToList();

            if (buffsToRemove.Count == 0)
                return;

            foreach (var buff in buffsToRemove)
            {
                await context.BuffsService.DeleteById(buff.Id);
                
                var buffLostMessage = new UnitLosesBuffCommandFromServer(
                    command.TurnIndex,
                    command.Player,
                    target.Id.ToString())
                {
                    BuffId = buff.Id.ToString(),
                    BuffName = buff.BuffType?.Name ?? "Unknown"
                };
                await context.MessageBroadcaster.BroadcastMessageAsync(buffLostMessage);
            }

            // Update in-memory buffs list
            var remainingBuffs = target.Buffs.Where(b => b.BuffType?.DeclinesWhenTakesDamage != true).ToList();
            target.Buffs = remainingBuffs;
        }

        private static async Task ApplyAttackBuffsToTarget(
            CommandExecutionContext context,
            MutableBattleUnitObject target,
            IAttackFunctionType attackType,
            UnitAttackClientBattleMessage command)
        {
            if (attackType.ApplyBuffTypeIds == null || attackType.ApplyBuffTypeIds.Count == 0)
                return;

            var newBuffs = new List<IBuffObject>();
            var replacedBuffTypeIds = new List<System.Guid>();
            
            for (int i = 0; i < attackType.ApplyBuffTypeIds.Count; i++)
            {
                var buffTypeId = attackType.ApplyBuffTypeIds[i];
                
                if (buffTypeId == System.Guid.Empty)
                    continue;
                
                // Get the chance for this buff (default to 100% if not specified)
                var chance = (attackType.ApplyBuffChances != null && i < attackType.ApplyBuffChances.Count) 
                    ? attackType.ApplyBuffChances[i] 
                    : 100;
                
                // Roll for buff application
                if (chance < 100)
                {
                    var roll = context.RandomProvider.NextInteger(0, 100);
                    if (roll >= chance)
                        continue; // Buff not applied
                }
                    
                var buffType = await context.BuffTypesService.GetById(buffTypeId);
                if (buffType == null)
                    continue;
                
                var durationRemaining = buffType.Permanent ? 0 : buffType.Duration;
                // BuffsService.Create will automatically remove any existing buff of the same type
                var buff = await context.BuffsService.Create(target.Id, buffType.Id, durationRemaining);
                newBuffs.Add(buff);
                replacedBuffTypeIds.Add(buffTypeId);
                
                var buffAppliedMessage = new UnitReceivesBuffCommandFromServer(
                    command.TurnIndex, 
                    command.Player, 
                    target.Id.ToString())
                {
                    BuffId = buff.Id.ToString(),
                    BuffName = buffType.Name,
                    ThumbnailUrl = buffType.ThumbnailUrl,
                    Permanent = buffType.Permanent,
                    DurationRemaining = durationRemaining,
                    Stunned = buffType.Stunned
                };
                await context.MessageBroadcaster.BroadcastMessageAsync(buffAppliedMessage);
            }
            
            // Update the in-memory buffs list for subsequent calculations in this battle session
            if (newBuffs.Count > 0)
            {
                // Remove existing buffs of the same types (they were replaced in the repository)
                var existingBuffs = target.Buffs?.ToList() ?? new List<IBuffObject>();
                existingBuffs.RemoveAll(b => replacedBuffTypeIds.Contains(b.BuffTypeId));
                existingBuffs.AddRange(newBuffs);
                target.Buffs = existingBuffs;
            }
        }

        private static async Task ApplyVampireHealing(
            CommandExecutionContext context,
            MutableBattleUnitObject attacker,
            int damageDealt,
            UnitAttackClientBattleMessage command)
        {
            // Calculate total vampire percentage from all buffs
            var vampirePercentage = 0;
            var canResurrect = false;
            
            if (attacker.Buffs != null)
            {
                foreach (var buff in attacker.Buffs)
                {
                    if (buff.BuffType != null && buff.BuffType.VampirePercentage > 0)
                    {
                        vampirePercentage += buff.BuffType.VampirePercentage;
                        if (buff.BuffType.VampireCanResurrect)
                            canResurrect = true;
                    }
                }
            }
            
            if (vampirePercentage <= 0)
                return;
            
            // Calculate HP to heal
            var hpToHeal = damageDealt * vampirePercentage / 100;
            if (hpToHeal <= 0)
                return;
            
            var unitHealth = attacker.GlobalUnit.UnitType.Health;
            var initialCount = attacker.InitialCount;
            var currentCount = attacker.CurrentCount;
            var currentHealth = attacker.CurrentHealth;
            
            // Calculate new health values
            var totalCurrentHp = (currentCount - 1) * unitHealth + currentHealth;
            var totalNewHp = totalCurrentHp + hpToHeal;
            
            // Cap at initial army size if resurrection is not allowed
            var maxTotalHp = canResurrect 
                ? initialCount * unitHealth 
                : currentCount * unitHealth;
            
            if (totalNewHp > maxTotalHp)
                totalNewHp = maxTotalHp;
            
            // Calculate new count and health
            var newCount = (totalNewHp + unitHealth - 1) / unitHealth; // Ceiling division
            var newHealth = totalNewHp - (newCount - 1) * unitHealth;
            
            // Ensure we don't exceed initial count
            if (newCount > initialCount)
            {
                newCount = initialCount;
                newHealth = unitHealth;
            }
            
            var actualHealedAmount = totalNewHp - totalCurrentHp;
            var resurrectedCount = newCount - currentCount;
            
            if (actualHealedAmount <= 0)
                return;
            
            // Update the attacker's state
            attacker.CurrentCount = newCount;
            attacker.CurrentHealth = newHealth;
            attacker.GlobalUnit.Count = newCount;
            
            await context.GlobalUnitsService.UpdateUnits(new[] { attacker.GlobalUnit });
            await context.BattleUnitsService.UpdateUnits(new[] { attacker });
            
            // Broadcast healing message
            var healMessage = new UnitHealsCommandFromServer(
                command.TurnIndex,
                command.Player,
                attacker.Id.ToString())
            {
                HealedAmount = actualHealedAmount,
                ResurrectedCount = resurrectedCount,
                NewCount = newCount,
                NewHealth = newHealth
            };
            await context.MessageBroadcaster.BroadcastMessageAsync(healMessage);
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