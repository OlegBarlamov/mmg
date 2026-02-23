using System;
using System.Collections.Generic;
using Epic.Data.MagicType;

namespace Epic.Core.Services.MagicTypes
{
    public interface IMagicTypesRegistry
    {
        IReadOnlyList<IMagicTypeEntity> All { get; }

        IMagicTypeEntity ById(Guid id);
        IMagicTypeEntity ByKey(string key);
    }
}
