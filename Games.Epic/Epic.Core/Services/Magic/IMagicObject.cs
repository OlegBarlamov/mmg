using System;
using System.Collections.Generic;
using Epic.Core.Services.Effects;
using Epic.Core.Services.MagicTypes;

namespace Epic.Core.Services.Magic
{
    public interface IMagicObject
    {
        Guid MagicTypeId { get; }
        IMagicTypeObject MagicType { get; }
        IReadOnlyList<EvaluatedBuffToApply> ApplyBuffs { get; }
        IReadOnlyList<IEffectObject> ApplyEffects { get; }
    }
}
