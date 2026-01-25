using System;
using System.Collections.Generic;
using Epic.Data.ArtifactType;

namespace Epic.Core.Services.ArtifactTypes
{
    public interface IArtifactTypesRegistry
    {
        IReadOnlyList<IArtifactTypeEntity> AllOrderedByValue { get; }

        IArtifactTypeEntity ById(Guid typeId);
        IArtifactTypeEntity ByKey(string key);
    }
}

