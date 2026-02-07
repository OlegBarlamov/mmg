using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core;
using Epic.Core.ServerMessages;
using Epic.Core.Services.Battles;
using Epic.Core.Services.Buffs;
using Epic.Core.Services.BuffTypes;
using Epic.Core.Services.GameManagement;
using Epic.Core.Services.Units;
using Epic.Data.UnitTypes.Subtypes;
using FrameworkSDK.Common;
using static Epic.Logic.Battle.BattleUnitBuffExtensions;

namespace Epic.Logic.Battle
{
    public interface IBuffsLogic
    {
        Task ProcessActiveUnitBuffs(
            MutableBattleUnitObject activeUnit,
            int turnNumber,
            InBattlePlayerNumber playerNumber);
        
        Task RemoveBuffsThatDeclineOnDamage(
            MutableBattleUnitObject target,
            int turnIndex,
            InBattlePlayerNumber player);
        
        Task ApplyAttackBuffsToTarget(
            MutableBattleUnitObject target,
            IAttackFunctionType attackType,
            int turnIndex,
            InBattlePlayerNumber player);
    }

    public class BuffsLogic : IBuffsLogic
    {
        private readonly IBuffsService _buffsService;
        private readonly IBuffTypesService _buffTypesService;
        private readonly IGlobalUnitsService _globalUnitsService;
        private readonly IBattleUnitsService _battleUnitsService;
        private readonly IRandomService _randomProvider;
        private readonly IBattleMessageBroadcaster _messageBroadcaster;
        private readonly IEffectsLogic _effectsLogic;

        public BuffsLogic(
            IBuffsService buffsService,
            IBuffTypesService buffTypesService,
            IGlobalUnitsService globalUnitsService,
            IBattleUnitsService battleUnitsService,
            IRandomService randomProvider,
            IBattleMessageBroadcaster messageBroadcaster,
            IEffectsLogic effectsLogic)
        {
            _buffsService = buffsService;
            _buffTypesService = buffTypesService;
            _globalUnitsService = globalUnitsService;
            _battleUnitsService = battleUnitsService;
            _randomProvider = randomProvider;
            _messageBroadcaster = messageBroadcaster;
            _effectsLogic = effectsLogic ?? throw new ArgumentNullException(nameof(effectsLogic));
        }

        public async Task ProcessActiveUnitBuffs(
            MutableBattleUnitObject activeUnit,
            int turnNumber,
            InBattlePlayerNumber playerNumber)
        {
            if (activeUnit?.Buffs == null || activeUnit.Buffs.Count == 0)
                return;

            var buffsToUpdate = new List<IBuffObject>();
            var buffsToRemove = new List<IBuffObject>();
            var remainingBuffs = new List<IBuffObject>();

            foreach (var buff in activeUnit.Buffs)
            {
                if (buff.BuffType?.Permanent == true)
                {
                    remainingBuffs.Add(buff);
                    continue;
                }

                if (buff is MutableBuffObject mutableBuff)
                {
                    mutableBuff.DurationRemaining--;
                    
                    if (mutableBuff.DurationRemaining < 0)
                    {
                        buffsToRemove.Add(buff);
                    }
                    else
                    {
                        buffsToUpdate.Add(buff);
                        remainingBuffs.Add(buff);
                    }
                }
            }

            if (buffsToUpdate.Count > 0)
            {
                await _buffsService.UpdateBatch(buffsToUpdate.ToArray());
            }

            foreach (var expiredBuff in buffsToRemove)
            {
                await _buffsService.DeleteById(expiredBuff.Id);
                
                var buffExpiredMessage = new UnitLosesBuffCommandFromServer(
                    turnNumber,
                    playerNumber,
                    activeUnit.Id.ToString())
                {
                    BuffId = expiredBuff.Id.ToString(),
                    BuffName = expiredBuff.BuffType?.Name ?? "Unknown"
                };
                await _messageBroadcaster.BroadcastMessageAsync(buffExpiredMessage);
            }

            if (buffsToRemove.Count > 0)
            {
                activeUnit.Buffs = remainingBuffs;
            }

            await _effectsLogic.ApplyEffects(
                activeUnit.GetEffectTypeProperties(),
                _randomProvider.NextInteger,
                activeUnit,
                turnNumber,
                playerNumber);
        }

        public async Task RemoveBuffsThatDeclineOnDamage(
            MutableBattleUnitObject target,
            int turnIndex,
            InBattlePlayerNumber player)
        {
            var buffsToRemove = target.GetBuffsToRemoveOnDamage().ToList();
            if (buffsToRemove.Count == 0)
                return;

            foreach (var buff in buffsToRemove)
            {
                await _buffsService.DeleteById(buff.Id);
                
                var buffLostMessage = new UnitLosesBuffCommandFromServer(
                    turnIndex,
                    player,
                    target.Id.ToString())
                {
                    BuffId = buff.Id.ToString(),
                    BuffName = buff.BuffType?.Name ?? "Unknown"
                };
                await _messageBroadcaster.BroadcastMessageAsync(buffLostMessage);
            }

            target.Buffs = target.GetBuffsRemainingAfterDamage();
        }

        public async Task ApplyAttackBuffsToTarget(
            MutableBattleUnitObject target,
            IAttackFunctionType attackType,
            int turnIndex,
            InBattlePlayerNumber player)
        {
            if (attackType.ApplyBuffTypeIds == null || attackType.ApplyBuffTypeIds.Count == 0)
                return;

            var newBuffs = new List<IBuffObject>();
            var replacedBuffTypeIds = new List<Guid>();
            
            for (int i = 0; i < attackType.ApplyBuffTypeIds.Count; i++)
            {
                var buffTypeId = attackType.ApplyBuffTypeIds[i];
                
                if (buffTypeId == Guid.Empty)
                    continue;
                
                var chance = (attackType.ApplyBuffChances != null && i < attackType.ApplyBuffChances.Count) 
                    ? attackType.ApplyBuffChances[i] 
                    : 100;
                
                if (chance < 100)
                {
                    var roll = _randomProvider.NextInteger(0, 100);
                    if (roll >= chance)
                        continue;
                }
                    
                var buffType = await _buffTypesService.GetById(buffTypeId);
                if (buffType == null)
                    continue;
                
                var durationRemaining = buffType.Permanent ? 0 : buffType.Duration;
                var buff = await _buffsService.Create(target.Id, buffType.Id, durationRemaining);
                newBuffs.Add(buff);
                replacedBuffTypeIds.Add(buffTypeId);
                
                var buffAppliedMessage = new UnitReceivesBuffCommandFromServer(
                    turnIndex, 
                    player, 
                    target.Id.ToString())
                {
                    BuffId = buff.Id.ToString(),
                    BuffTypeId = buffType.Id.ToString(),
                    BuffName = buffType.Name,
                    ThumbnailUrl = buffType.ThumbnailUrl,
                    Permanent = buffType.Permanent,
                    Stunned = buffType.Stunned,
                    Paralyzed = buffType.Paralyzed,
                    DurationRemaining = durationRemaining,
                };
                await _messageBroadcaster.BroadcastMessageAsync(buffAppliedMessage);
            }
            
            if (newBuffs.Count > 0)
            {
                var existingBuffs = target.Buffs?.ToList() ?? new List<IBuffObject>();
                existingBuffs.RemoveAll(b => replacedBuffTypeIds.Contains(b.BuffTypeId));
                existingBuffs.AddRange(newBuffs);
                target.Buffs = existingBuffs;
            }
        }

    }
}
