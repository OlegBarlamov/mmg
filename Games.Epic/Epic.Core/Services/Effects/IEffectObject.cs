using System;
using Epic.Core.Services.EffectTypes;
using Epic.Data.Effect;
using Epic.Data.EffectType;

namespace Epic.Core.Services.Effects
{
    public interface IEffectObject : IEffectProperties
    {
        Guid EffectTypeId { get; }
        IEffectTypeObject EffectType { get; }
        EffectAnimation Animation { get; }
        int AnimationTimeMs { get; }
    }
}
