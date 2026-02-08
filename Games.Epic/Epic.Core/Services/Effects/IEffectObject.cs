using System;
using Epic.Core.Services.EffectTypes;
using Epic.Data.Effect;

namespace Epic.Core.Services.Effects
{
    public interface IEffectObject : IEffectProperties
    {
        Guid EffectTypeId { get; }
        IEffectTypeObject EffectType { get; }
    }
}
