using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.EffectType;
using JetBrains.Annotations;

namespace Epic.Core.Services.EffectTypes
{
    [UsedImplicitly]
    public class DefaultEffectTypesService : IEffectTypesService
    {
        public IEffectTypesRepository Repository { get; }

        public DefaultEffectTypesService([NotNull] IEffectTypesRepository repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IEffectTypeObject> GetById(Guid id)
        {
            var entity = await Repository.GetById(id);
            return MutableEffectTypeObject.FromEntity(entity);
        }

        public async Task<IEffectTypeObject> GetByKey(string key)
        {
            var entity = await Repository.GetByKey(key);
            return entity == null ? null : MutableEffectTypeObject.FromEntity(entity);
        }

        public async Task<IEffectTypeObject[]> GetAll()
        {
            var entities = await Repository.GetAll();
            return entities.Select(MutableEffectTypeObject.FromEntity).ToArray<IEffectTypeObject>();
        }
    }
}
