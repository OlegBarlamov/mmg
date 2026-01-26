using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.BuffTypes;
using Epic.Data.ArtifactType;
using JetBrains.Annotations;

namespace Epic.Core.Services.ArtifactTypes
{
    [UsedImplicitly]
    public class DefaultArtifactTypesService : IArtifactTypesService
    {
        public IArtifactTypesRepository Repository { get; }
        public IBuffTypesService BuffTypesService { get; }

        public DefaultArtifactTypesService(
            [NotNull] IArtifactTypesRepository repository,
            [NotNull] IBuffTypesService buffTypesService)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            BuffTypesService = buffTypesService ?? throw new ArgumentNullException(nameof(buffTypesService));
        }

        public async Task<IArtifactTypeObject> GetById(Guid id)
        {
            var entity = await Repository.GetById(id);
            var obj = MutableArtifactTypeObject.FromEntity(entity);
            obj.BuffTypes = await Task.WhenAll((obj.BuffTypeIds ?? Array.Empty<Guid>()).Select(BuffTypesService.GetById));
            return obj;
        }

        public async Task<IArtifactTypeObject> GetByKey(string key)
        {
            var entity = await Repository.GetByKey(key);
            if (entity == null)
                return null;

            var obj = MutableArtifactTypeObject.FromEntity(entity);
            obj.BuffTypes = await Task.WhenAll((obj.BuffTypeIds ?? Array.Empty<Guid>()).Select(BuffTypesService.GetById));
            return obj;
        }

        public async Task<IArtifactTypeObject[]> GetAll()
        {
            var entities = await Repository.GetAll();
            var objects = entities.Select(MutableArtifactTypeObject.FromEntity).ToArray();
            await Task.WhenAll(objects.Select(async o =>
                o.BuffTypes = await Task.WhenAll((o.BuffTypeIds ?? Array.Empty<Guid>()).Select(BuffTypesService.GetById))));
            return objects.ToArray<IArtifactTypeObject>();
        }
    }
}

