using System;
using System.Threading.Tasks;

namespace Epic.Core.Services.ArtifactTypes
{
    public interface IArtifactTypesService
    {
        Task<IArtifactTypeObject> GetById(Guid id);
        Task<IArtifactTypeObject> GetByKey(string key);
        Task<IArtifactTypeObject[]> GetAll();
    }
}

