using System;
using System.Threading.Tasks;
using Epic.Data.GameResources;
using JetBrains.Annotations;

namespace Epic.Core.Services.GameResources
{
    [UsedImplicitly]
    public class DefaultGameResourcesService : IGameResourcesService
    {
        public IGameResourcesRepository GameResourcesRepository { get; }

        public DefaultGameResourcesService([NotNull] IGameResourcesRepository gameResourcesRepository)
        {
            GameResourcesRepository = gameResourcesRepository ?? throw new ArgumentNullException(nameof(gameResourcesRepository));
        }
        
        public async Task<ResourceAmount> CreateResourceAmount(Guid resourceId, int amount)
        {
            var entity = await GameResourcesRepository.GetById(resourceId);
            return ResourceAmount.Create(entity, amount);
        }
    }
}
