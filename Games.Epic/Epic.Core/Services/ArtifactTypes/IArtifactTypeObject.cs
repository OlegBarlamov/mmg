using System;
using System.Collections.Generic;
using Epic.Core.Objects;
using Epic.Core.Services.BuffTypes;
using Epic.Data.ArtifactType;
using Epic.Data.Artifacts;

namespace Epic.Core.Services.ArtifactTypes
{
    public interface IArtifactTypeObject : IGameObject<IArtifactTypeEntity>, IArtifactTypeEntityFields
    {
        Guid Id { get; }
        new ArtifactSlot[] Slots { get; }

        IReadOnlyList<IBuffTypeObject> BuffTypes { get; }
    }
}

