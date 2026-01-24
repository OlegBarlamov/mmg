using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.Exceptions;
using JetBrains.Annotations;

namespace Epic.Data.ArtifactType
{
    [UsedImplicitly]
    public class InMemoryArtifactTypesRepository : IArtifactTypesRepository
    {
        public string Name => nameof(InMemoryArtifactTypesRepository);
        public string EntityName => "ArtifactType";

        private readonly List<ArtifactTypeEntity> _types = new List<ArtifactTypeEntity>();

        public Task<IArtifactTypeEntity> GetById(Guid id)
        {
            var entity = _types.FirstOrDefault(x => x.Id == id);
            if (entity == null)
                throw new EntityNotFoundException(this, id.ToString());
            return Task.FromResult<IArtifactTypeEntity>(entity);
        }

        public Task<IArtifactTypeEntity> GetByKey(string key)
        {
            return Task.FromResult<IArtifactTypeEntity>(_types.FirstOrDefault(x => x.Key == key));
        }

        public Task<IArtifactTypeEntity[]> GetAll()
        {
            return Task.FromResult(_types.ToArray<IArtifactTypeEntity>());
        }

        public Task<IArtifactTypeEntity> Create(Guid id, IArtifactTypeEntityFields fields)
        {
            var entity = ArtifactTypeEntity.FromFields(id, fields);
            _types.Add(entity);
            return Task.FromResult<IArtifactTypeEntity>(entity);
        }

        public Task<IArtifactTypeEntity[]> CreateBatch(IArtifactTypeEntityFields[] fields)
        {
            var entities = fields.Select(x => ArtifactTypeEntity.FromFields(Guid.NewGuid(), x)).ToArray();
            _types.AddRange(entities);
            return Task.FromResult(entities.ToArray<IArtifactTypeEntity>());
        }

        public Task UpdateBatch(IArtifactTypeEntity[] entities)
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

