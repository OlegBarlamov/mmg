using System;
using System.Collections.Generic;
using Epic.Data.BuffType;

namespace Epic.Core.Services.BuffTypes
{
    public interface IBuffTypesRegistry
    {
        IReadOnlyList<IBuffTypeEntity> All { get; }

        IBuffTypeEntity ById(Guid id);
        IBuffTypeEntity ByKey(string key);
    }
}

