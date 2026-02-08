using System;
using System.Threading.Tasks;
using Epic.Core.Services.EffectTypes;
using JetBrains.Annotations;

namespace Epic.Core.Services.Effects
{
    [UsedImplicitly]
    public class DefaultEffectsService : IEffectsService
    {
        public IEffectTypesService EffectTypesService { get; }

        public DefaultEffectsService([NotNull] IEffectTypesService effectTypesService)
        {
            EffectTypesService = effectTypesService ?? throw new ArgumentNullException(nameof(effectTypesService));
        }

        public async Task<IEffectObject> Create(Guid effectTypeId, EffectExpressionsVariables variables = null)
        {
            if (effectTypeId == Guid.Empty) throw new ArgumentException("EffectTypeId must not be empty.", nameof(effectTypeId));

            var type = await EffectTypesService.GetById(effectTypeId);
            var vars = variables?.ToDictionary();
            var effectiveValues = EffectExpressionEvaluator.Evaluate(type, vars);

            var obj = MutableEffectObject.Create(effectTypeId, effectiveValues);
            obj.EffectType = type;
            return obj;
        }
    }
}
