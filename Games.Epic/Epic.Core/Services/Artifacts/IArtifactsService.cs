using System;
using System.Threading.Tasks;

namespace Epic.Core.Services.Artifacts
{
    public interface IArtifactsService
    {
        Task<IArtifactObject> GetById(Guid id);
        Task<IArtifactObject[]> GetByHeroId(Guid heroId);

        Task<IArtifactObject> Create(Guid heroId, Guid typeId);
        Task<IArtifactObject> EquipArtifact(Guid heroId, IArtifactObject artifact, int[] equippedSlotsIndexes);
        Task Update(IArtifactObject artifact);
        Task Remove(Guid id);
    }
}

