using System;
using System.Threading.Tasks;

namespace Epic.Data.Artifact
{
    public interface IArtifactsRepository : IRepository
    {
        Task<IArtifactEntity> GetById(Guid id);
        Task<IArtifactEntity[]> GetByHeroId(Guid heroId);
        
        Task<IArtifactEntity> Create(Guid id, IArtifactFields fields);
        Task Update(IArtifactEntity entity);
        Task Remove(Guid id);
    }
}

