using System;
using Epic.Core.Services.ArtifactTypes;
using Epic.Core.Objects;
using Epic.Data.Artifact;

namespace Epic.Core.Services.Artifacts
{
    public interface IArtifactObject : IGameObject<IArtifactEntity>, IArtifactFields
    {
        Guid Id { get; }

        IArtifactTypeObject ArtifactType { get; }
    }
}