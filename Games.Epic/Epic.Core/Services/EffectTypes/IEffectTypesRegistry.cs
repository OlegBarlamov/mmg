using System;
using System.Collections.Generic;
using Epic.Data.EffectType;

namespace Epic.Core.Services.EffectTypes
{
    public interface IEffectTypesRegistry
    {
        IReadOnlyList<IEffectTypeEntity> All { get; }

        IEffectTypeEntity ById(Guid id);
        IEffectTypeEntity ByKey(string key);
    }
}
