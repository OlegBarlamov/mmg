using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epic.Core.Services.Buffs;
using Epic.Core.Services.Effects;
using Epic.Core.Services.MagicTypes;
using JetBrains.Annotations;

namespace Epic.Core.Services.Magic
{
    [UsedImplicitly]
    public class DefaultMagicsService : IMagicsService
    {
        public IMagicTypesService MagicTypesService { get; }
        public IEffectsService EffectsService { get; }

        public DefaultMagicsService(
            [NotNull] IMagicTypesService magicTypesService,
            [NotNull] IEffectsService effectsService)
        {
            MagicTypesService = magicTypesService ?? throw new ArgumentNullException(nameof(magicTypesService));
            EffectsService = effectsService ?? throw new ArgumentNullException(nameof(effectsService));
        }

        public async Task<IMagicObject> Create(Guid magicTypeId, MagicExpressionsVariables variables = null)
        {
            if (magicTypeId == Guid.Empty)
                throw new ArgumentException("MagicTypeId must not be empty.", nameof(magicTypeId));

            var magicType = await MagicTypesService.GetById(magicTypeId);
            var buffVars = variables?.ToBuffExpressionsVariables();
            var effectVars = variables?.ToEffectExpressionsVariables();

            var applyBuffs = new List<EvaluatedBuffToApply>(magicType.ApplyBuffsTypes.Count);
            foreach (var buffType in magicType.ApplyBuffsTypes)
            {
                var effectiveValues = BuffExpressionEvaluator.Evaluate(buffType, buffVars);
                applyBuffs.Add(new EvaluatedBuffToApply
                {
                    BuffType = buffType,
                    EffectiveValues = effectiveValues,
                });
            }

            var applyEffects = new List<IEffectObject>(magicType.EffectTypes.Count);
            foreach (var effectType in magicType.EffectTypes)
            {
                var effectObj = await EffectsService.Create(effectType.Id, effectVars);
                applyEffects.Add(effectObj);
            }

            return new MutableMagicObject
            {
                MagicTypeId = magicTypeId,
                MagicType = magicType,
                ApplyBuffs = applyBuffs,
                ApplyEffects = applyEffects,
            };
        }
    }
}
