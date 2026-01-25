using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.ArtifactType;
using JetBrains.Annotations;

namespace Epic.Core.Services.ArtifactTypes
{
    [UsedImplicitly]
    public class DefaultArtifactTypesService : IArtifactTypesService
    {
        public IArtifactTypesRepository Repository { get; }

        public DefaultArtifactTypesService([NotNull] IArtifactTypesRepository repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IArtifactTypeObject> GetById(Guid id)
        {
            var entity = await Repository.GetById(id);
            return MutableArtifactTypeObject.FromEntity(entity);
        }

        public async Task<IArtifactTypeObject> GetByKey(string key)
        {
            var entity = await Repository.GetByKey(key);
            return entity == null ? null : MutableArtifactTypeObject.FromEntity(entity);
        }

        public async Task<IArtifactTypeObject[]> GetAll()
        {
            var entities = await Repository.GetAll();
            return entities.Select(MutableArtifactTypeObject.FromEntity).ToArray<IArtifactTypeObject>();
        }
    }
}

