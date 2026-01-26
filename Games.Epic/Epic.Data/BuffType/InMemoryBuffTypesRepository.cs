using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.Exceptions;
using JetBrains.Annotations;

namespace Epic.Data.BuffType
{
    [UsedImplicitly]
    public class InMemoryBuffTypesRepository : IBuffTypesRepository
    {
        public string Name => nameof(InMemoryBuffTypesRepository);
        public string EntityName => "BuffType";

        private readonly List<BuffTypeEntity> _types = new List<BuffTypeEntity>();

        public Task<IBuffTypeEntity> GetById(Guid id)
        {
            var entity = _types.FirstOrDefault(x => x.Id == id);
            if (entity == null)
                throw new EntityNotFoundException(this, id.ToString());
            return Task.FromResult<IBuffTypeEntity>(entity);
        }

        public Task<IBuffTypeEntity> GetByKey(string key)
        {
            return Task.FromResult<IBuffTypeEntity>(_types.FirstOrDefault(x => x.Key == key));
        }

        public Task<IBuffTypeEntity[]> GetAll()
        {
            return Task.FromResult(_types.ToArray<IBuffTypeEntity>());
        }

        public Task<IBuffTypeEntity> Create(Guid id, IBuffFields fields)
        {
            var entity = BuffTypeEntity.FromFields(id, fields);
            _types.Add(entity);
            return Task.FromResult<IBuffTypeEntity>(entity);
        }

        public Task<IBuffTypeEntity[]> CreateBatch(IBuffFields[] fields)
        {
            var entities = fields.Select(x => BuffTypeEntity.FromFields(Guid.NewGuid(), x)).ToArray();
            _types.AddRange(entities);
            return Task.FromResult(entities.ToArray<IBuffTypeEntity>());
        }

        public Task UpdateBatch(IBuffTypeEntity[] entities)
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

