using System;
using System.Collections.Generic;
using Epic.Core.Services.Effects;
using Epic.Core.Services.MagicTypes;

namespace Epic.Core.Services.Magic
{
    public class MutableMagicObject : IMagicObject
    {
        public Guid MagicTypeId { get; set; }
        public IMagicTypeObject MagicType { get; set; }
        public IReadOnlyList<EvaluatedBuffToApply> ApplyBuffs { get; set; }
        public IReadOnlyList<IEffectObject> ApplyEffects { get; set; }

        internal MutableMagicObject() { }
    }
}
