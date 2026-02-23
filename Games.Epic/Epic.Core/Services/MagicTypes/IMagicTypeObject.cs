using System;
using System.Collections.Generic;
using Epic.Core.Objects;
using Epic.Core.Services.BuffTypes;
using Epic.Core.Services.EffectTypes;
using Epic.Data.MagicType;

namespace Epic.Core.Services.MagicTypes
{
    public interface IMagicTypeObject : IGameObject<IMagicTypeEntity>, IMagicTypeFields
    {
        Guid Id { get; }
        IReadOnlyList<IBuffTypeObject> ApplyBuffsTypes { get; }
        IReadOnlyList<IEffectTypeObject> EffectTypes { get; }
    }
}
