using System;
using Epic.Core.Services.EffectTypes;
using Epic.Data.Effect;
using Epic.Data.EffectType;

namespace Epic.Core.Services.Effects
{
    /// <summary>
    /// In-memory effect instance with evaluated effective values. No identity, target, or duration.
    /// </summary>
    public class MutableEffectObject : IEffectObject
    {
        public Guid EffectTypeId { get; set; }
        public IEffectTypeObject EffectType { get; set; }

        private readonly IEffectProperties _effectiveValues;

        public int TakesDamageMin => _effectiveValues?.TakesDamageMin ?? 0;
        public int TakesDamageMax => _effectiveValues?.TakesDamageMax ?? 0;
        public int Heals => _effectiveValues?.Heals ?? 0;
        public int HealsPercentage => _effectiveValues?.HealsPercentage ?? 0;
        public bool HealCanResurrect => _effectiveValues?.HealCanResurrect ?? false;
        public EffectAnimation Animation => EffectType?.Animation ?? EffectAnimation.None;
        public int AnimationTimeMs => EffectType?.AnimationTimeMs ?? 0;

        private MutableEffectObject(IEffectProperties effectiveValues)
        {
            _effectiveValues = effectiveValues;
        }

        /// <summary>
        /// Creates an in-memory effect with the given type and evaluated effective values.
        /// </summary>
        public static MutableEffectObject Create(Guid effectTypeId, IEffectProperties effectiveValues)
        {
            return new MutableEffectObject(effectiveValues ?? new EffectEffectiveValues())
            {
                EffectTypeId = effectTypeId,
                EffectType = null,
            };
        }
    }
}
