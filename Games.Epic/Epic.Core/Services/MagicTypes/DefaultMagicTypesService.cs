using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core.Services.BuffTypes;
using Epic.Core.Services.EffectTypes;
using Epic.Data.MagicType;
using JetBrains.Annotations;

namespace Epic.Core.Services.MagicTypes
{
    [UsedImplicitly]
    public class DefaultMagicTypesService : IMagicTypesService
    {
        public IMagicTypesRepository Repository { get; }
        public IBuffTypesService BuffTypesService { get; }
        public IEffectTypesService EffectTypesService { get; }

        public DefaultMagicTypesService(
            [NotNull] IMagicTypesRepository repository,
            [NotNull] IBuffTypesService buffTypesService,
            [NotNull] IEffectTypesService effectTypesService)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            BuffTypesService = buffTypesService ?? throw new ArgumentNullException(nameof(buffTypesService));
            EffectTypesService = effectTypesService ?? throw new ArgumentNullException(nameof(effectTypesService));
        }

        public async Task<IMagicTypeObject> GetById(Guid id)
        {
            var entity = await Repository.GetById(id);
            return await MutableMagicTypeObject.FromEntityAsync(entity, BuffTypesService, EffectTypesService);
        }

        public async Task<IMagicTypeObject> GetByKey(string key)
        {
            var entity = await Repository.GetByKey(key);
            return entity == null ? null : await MutableMagicTypeObject.FromEntityAsync(entity, BuffTypesService, EffectTypesService);
        }

        public async Task<IMagicTypeObject[]> GetAll()
        {
            var entities = await Repository.GetAll();
            var result = new IMagicTypeObject[entities.Length];
            for (var i = 0; i < entities.Length; i++)
                result[i] = await MutableMagicTypeObject.FromEntityAsync(entities[i], BuffTypesService, EffectTypesService);
            return result;
        }

        public async Task<IMagicTypeObject[]> GetByIds(Guid[] ids)
        {
            if (ids == null || ids.Length == 0)
                return Array.Empty<IMagicTypeObject>();
            var tasks = ids.Select(id => GetById(id)).ToArray();
            return await Task.WhenAll(tasks);
        }
    }
}
