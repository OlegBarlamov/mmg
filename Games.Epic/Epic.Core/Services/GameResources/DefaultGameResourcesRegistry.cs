using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Epic.Data.GameResources;
using JetBrains.Annotations;

namespace Epic.Core.Services.GameResources
{
    [UsedImplicitly]
    public class DefaultGameResourcesRegistry : IGameResourcesRegistry
    {
        private IGameResourcesRepository GameResourcesRepository { get; }

        private readonly List<IGameResourceEntity> _resources = new List<IGameResourceEntity>();

        public DefaultGameResourcesRegistry([NotNull] IGameResourcesRepository gameResourcesRepository)
        {
            GameResourcesRepository = gameResourcesRepository ?? throw new ArgumentNullException(nameof(gameResourcesRepository));
        }
        
        public IReadOnlyCollection<IGameResourceEntity> GetAll()
        {
            return _resources;
        }

        public async Task Load(CancellationToken cancellationToken)
        {
            _resources.Clear();
            
            var resourcesByKeys = await GameResourcesRepository.GetAllResourcesByKeys();
            _resources.AddRange(resourcesByKeys.Values);
        }
    }
}