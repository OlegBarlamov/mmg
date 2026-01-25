using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.Exceptions;
using JetBrains.Annotations;

namespace Epic.Data.Artifact
{
    [UsedImplicitly]
    public class InMemoryArtifactsRepository : IArtifactsRepository
    {
        public string Name => nameof(InMemoryArtifactsRepository);
        public string EntityName => "Artifact";

        private readonly List<ArtifactEntity> _artifacts = new List<ArtifactEntity>();

        public Task<IArtifactEntity> GetById(Guid id)
        {
            var entity = _artifacts.FirstOrDefault(x => x.Id == id);
            if (entity == null)
                throw new EntityNotFoundException(this, id.ToString());
            return Task.FromResult<IArtifactEntity>(entity);
        }

        public Task<IArtifactEntity[]> GetByHeroId(Guid heroId)
        {
            var result = _artifacts.Where(x => x.HeroId == heroId).ToArray<IArtifactEntity>();
            return Task.FromResult(result);
        }

        public Task<IArtifactEntity> Create(Guid id, IArtifactFields fields)
        {
            var entity = ArtifactEntity.FromFields(id, fields);
            _artifacts.Add(entity);
            return Task.FromResult<IArtifactEntity>(entity);
        }

        public Task Update(IArtifactEntity entity)
        {
            var existing = _artifacts.FirstOrDefault(x => x.Id == entity.Id);
            if (existing == null)
                throw new EntityNotFoundException(this, entity.Id.ToString());
            
            existing.FillFrom(entity);
            return Task.CompletedTask;
        }

        public Task Remove(Guid id)
        {
            var existing = _artifacts.FirstOrDefault(x => x.Id == id);
            if (existing == null)
                throw new EntityNotFoundException(this, id.ToString());

            _artifacts.Remove(existing);
            return Task.CompletedTask;
        }
    }
}

