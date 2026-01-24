using System;
using System.Threading.Tasks;

namespace Epic.Data.ArtifactType
{
    public interface IArtifactTypesRepository : IRepository
    {
        Task<IArtifactTypeEntity> GetById(Guid id);
        Task<IArtifactTypeEntity> GetByKey(string key);
        Task<IArtifactTypeEntity[]> GetAll();
        
        Task<IArtifactTypeEntity> Create(Guid id, IArtifactTypeEntityFields fields);
        Task<IArtifactTypeEntity[]> CreateBatch(IArtifactTypeEntityFields[] fields);
        Task UpdateBatch(IArtifactTypeEntity[] entities);
    }
}

