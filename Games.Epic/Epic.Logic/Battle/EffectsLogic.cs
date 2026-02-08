using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epic.Core;
using Epic.Core.ServerMessages;
using Epic.Core.Services.Battles;
using Epic.Core.Services.GameManagement;
using Epic.Core.Services.Units;
using Epic.Data.Effect;

namespace Epic.Logic.Battle
{
    public interface IEffectsLogic
    {
        Task ApplyEffects(
            IEnumerable<IEffectProperties> effectProperties,
            Func<int, int, int> randomInRange,
            MutableBattleUnitObject unit,
            int turnNumber,
            InBattlePlayerNumber playerNumber);
    }
    
    public class EffectsLogic : IEffectsLogic
    {
        private readonly IGlobalUnitsService _globalUnitsService;
        private readonly IBattleUnitsService _battleUnitsService;
        private readonly IBattleMessageBroadcaster _messageBroadcaster;

        public EffectsLogic(
            IGlobalUnitsService globalUnitsService,
            IBattleUnitsService battleUnitsService,
            IBattleMessageBroadcaster messageBroadcaster)
        {
            _globalUnitsService = globalUnitsService ?? throw new ArgumentNullException(nameof(globalUnitsService));
            _battleUnitsService = battleUnitsService ?? throw new ArgumentNullException(nameof(battleUnitsService));
            _messageBroadcaster = messageBroadcaster ?? throw new ArgumentNullException(nameof(messageBroadcaster));
        }

        public async Task ApplyEffects(
            IEnumerable<IEffectProperties> effectProperties,
            Func<int, int, int> randomInRange,
            MutableBattleUnitObject unit,
            int turnNumber,
            InBattlePlayerNumber playerNumber)
        {
            if (effectProperties == null)
                return;

            if (randomInRange == null)
                throw new ArgumentNullException(nameof(randomInRange));

            if (unit == null)
                throw new ArgumentNullException(nameof(unit));

            var flatHeal = 0;
            var percentageHeal = 0;
            var healCanResurrect = false;
            var dotDamage = 0;

            foreach (var source in effectProperties)
            {
                if (source == null)
                    continue;

                flatHeal += source.Heals;
                percentageHeal += source.HealsPercentage;
                if (source.HealCanResurrect)
                    healCanResurrect = true;

                var minDmg = source.TakesDamageMin;
                var maxDmg = source.TakesDamageMax;
                if (minDmg > 0 || maxDmg > 0)
                {
                    if (maxDmg < minDmg) maxDmg = minDmg;
                    dotDamage += randomInRange(minDmg, maxDmg + 1);
                }
            }

            await ApplyHealing(unit, flatHeal, percentageHeal, healCanResurrect, turnNumber, playerNumber);
            await ApplyDamage(unit, dotDamage, turnNumber, playerNumber);
        }

        private async Task ApplyHealing(
            MutableBattleUnitObject unit,
            int totalFlatHeal,
            int totalPercentageHeal,
            bool canResurrect,
            int turnNumber,
            InBattlePlayerNumber playerNumber)
        {
            if (totalFlatHeal <= 0 && totalPercentageHeal <= 0)
                return;

            var unitHealth = unit.GetEffectiveMaxHealth();
            var initialCount = unit.InitialCount;
            var currentCount = unit.CurrentCount;
            var currentHealth = unit.CurrentHealth;

            var percentageHeal = unitHealth * totalPercentageHeal / 100;
            var hpToHeal = totalFlatHeal + percentageHeal;

            if (hpToHeal <= 0)
                return;

            var totalCurrentHp = (currentCount - 1) * unitHealth + currentHealth;
            var totalNewHp = totalCurrentHp + hpToHeal;

            var maxTotalHp = canResurrect
                ? initialCount * unitHealth
                : currentCount * unitHealth;

            if (totalNewHp > maxTotalHp)
                totalNewHp = maxTotalHp;

            var newCount = (totalNewHp + unitHealth - 1) / unitHealth;
            var newHealth = totalNewHp - (newCount - 1) * unitHealth;

            if (newCount > initialCount)
            {
                newCount = initialCount;
                newHealth = unitHealth;
            }

            var actualHealedAmount = totalNewHp - totalCurrentHp;
            var resurrectedCount = newCount - currentCount;

            if (actualHealedAmount <= 0)
                return;

            unit.CurrentCount = newCount;
            unit.CurrentHealth = newHealth;
            unit.GlobalUnit.Count = newCount;

            await _globalUnitsService.UpdateUnits(new[] { unit.GlobalUnit });
            await _battleUnitsService.UpdateUnits(new[] { unit });

            var healMessage = new UnitHealsCommandFromServer(turnNumber, playerNumber, unit.Id.ToString())
            {
                HealedAmount = actualHealedAmount,
                ResurrectedCount = resurrectedCount,
                NewCount = newCount,
                NewHealth = newHealth
            };
            await _messageBroadcaster.BroadcastMessageAsync(healMessage);
        }

        private async Task ApplyDamage(
            MutableBattleUnitObject unit,
            int totalDamage,
            int turnNumber,
            InBattlePlayerNumber playerNumber)
        {
            if (totalDamage <= 0)
                return;

            var unitHealth = unit.GetEffectiveMaxHealth();
            var currentCount = unit.CurrentCount;
            var currentHealth = unit.CurrentHealth;

            var totalCurrentHp = (currentCount - 1) * unitHealth + currentHealth;
            var totalNewHp = totalCurrentHp - totalDamage;

            if (totalNewHp < 0)
                totalNewHp = 0;

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
                newCount = (totalNewHp + unitHealth - 1) / unitHealth;
                newHealth = totalNewHp - (newCount - 1) * unitHealth;
                killedCount = currentCount - newCount;
            }

            var actualDamageTaken = totalCurrentHp - totalNewHp;

            if (actualDamageTaken <= 0)
                return;

            unit.CurrentCount = newCount;
            unit.CurrentHealth = newHealth;
            unit.GlobalUnit.Count = newCount;

            await _globalUnitsService.UpdateUnits(new[] { unit.GlobalUnit });
            await _battleUnitsService.UpdateUnits(new[] { unit });

            var damageMessage = new UnitTakesDamageCommandFromServer(turnNumber, playerNumber, unit.Id.ToString())
            {
                DamageTaken = actualDamageTaken,
                KilledCount = killedCount,
                RemainingCount = newCount,
                RemainingHealth = newHealth
            };
            await _messageBroadcaster.BroadcastMessageAsync(damageMessage);

            if (newCount <= 0)
            {
                unit.GlobalUnit.IsAlive = false;
                await _globalUnitsService.UpdateUnits(new[] { unit.GlobalUnit });
            }
        }
    }
}
