using System;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.BuffType;
using JetBrains.Annotations;

namespace Epic.Core.Services.BuffTypes
{
    [UsedImplicitly]
    public class DefaultBuffTypesService : IBuffTypesService
    {
        public IBuffTypesRepository Repository { get; }

        public DefaultBuffTypesService([NotNull] IBuffTypesRepository repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IBuffTypeObject> GetById(Guid id)
        {
            var entity = await Repository.GetById(id);
            return MutableBuffTypeObject.FromEntity(entity);
        }

        public async Task<IBuffTypeObject> GetByKey(string key)
        {
            var entity = await Repository.GetByKey(key);
            return entity == null ? null : MutableBuffTypeObject.FromEntity(entity);
        }

        public async Task<IBuffTypeObject[]> GetAll()
        {
            var entities = await Repository.GetAll();
            return entities.Select(MutableBuffTypeObject.FromEntity).ToArray<IBuffTypeObject>();
        }
    }
}

