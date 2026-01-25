using System;
using Epic.Core.Objects;
using Epic.Data.ArtifactType;
using Epic.Data.Artifacts;

namespace Epic.Core.Services.ArtifactTypes
{
    public interface IArtifactTypeObject : IGameObject<IArtifactTypeEntity>, IArtifactTypeEntityFields
    {
        Guid Id { get; }
        new ArtifactSlot[] Slots { get; }
    }
}

