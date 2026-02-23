using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.Exceptions;
using JetBrains.Annotations;

namespace Epic.Data.MagicType
{
    [UsedImplicitly]
    public class InMemoryMagicTypesRepository : IMagicTypesRepository
    {
        public string Name => nameof(InMemoryMagicTypesRepository);
        public string EntityName => "MagicType";

        private readonly List<MagicTypeEntity> _types = new List<MagicTypeEntity>();

        public Task<IMagicTypeEntity> GetById(Guid id)
        {
            var entity = _types.FirstOrDefault(x => x.Id == id);
            if (entity == null)
                throw new EntityNotFoundException(this, id.ToString());
            return Task.FromResult<IMagicTypeEntity>(entity);
        }

        public Task<IMagicTypeEntity> GetByKey(string key)
        {
            return Task.FromResult<IMagicTypeEntity>(_types.FirstOrDefault(x => x.Key == key));
        }

        public Task<IMagicTypeEntity[]> GetAll()
        {
            return Task.FromResult(_types.ToArray<IMagicTypeEntity>());
        }

        public Task<IMagicTypeEntity> Create(Guid id, IMagicTypeFields fields)
        {
            var entity = MagicTypeEntity.FromFields(id, fields);
            _types.Add(entity);
            return Task.FromResult<IMagicTypeEntity>(entity);
        }

        public Task<IMagicTypeEntity[]> CreateBatch(IMagicTypeFields[] fields)
        {
            var entities = fields.Select(x => MagicTypeEntity.FromFields(Guid.NewGuid(), x)).ToArray();
            _types.AddRange(entities);
            return Task.FromResult(entities.ToArray<IMagicTypeEntity>());
        }

        public Task UpdateBatch(IMagicTypeEntity[] entities)
        {
            foreach (var entity in entities)
            {
                var existing = _types.FirstOrDefault(x => x.Id == entity.Id);
                if (existing == null)
                    throw new EntityNotFoundException(this, entity.Id.ToString());
                existing.FillFrom(entity);
            }
            return Task.CompletedTask;
        }
    }
}
