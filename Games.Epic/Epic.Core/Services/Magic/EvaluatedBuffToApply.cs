using Epic.Core.Services.BuffTypes;
using Epic.Data.Buff;
using JetBrains.Annotations;

namespace Epic.Core.Services.Magic
{
    /// <summary>
    /// A buff type with evaluated effective values, ready to be applied to a target (e.g. when casting magic).
    /// </summary>
    public class EvaluatedBuffToApply
    {
        [NotNull]
        public IBuffTypeObject BuffType { get; set; }

        [NotNull]
        public BuffEffectiveValues EffectiveValues { get; set; }
    }
}
