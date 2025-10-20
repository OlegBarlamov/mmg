using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Epic.Core.Services.GameResources;
using Epic.Data.GameResources;
using FrameworkSDK;
using JetBrains.Annotations;

namespace Epic.Server
{
    [UsedImplicitly]
    public class InitializeResourcesScript : IAppComponent
    {
        public IGameResourcesRepository GameResourcesRepository { get; }
        public DefaultGameResourcesRegistry DefaultGameResourcesRegistry { get; }

        public InitializeResourcesScript(
            [NotNull] IGameResourcesRepository gameResourcesRepository,
            [NotNull] DefaultGameResourcesRegistry defaultGameResourcesRegistry)
        {
            GameResourcesRepository = gameResourcesRepository ?? throw new ArgumentNullException(nameof(gameResourcesRepository));
            DefaultGameResourcesRegistry = defaultGameResourcesRegistry ?? throw new ArgumentNullException(nameof(defaultGameResourcesRegistry));
        }
        
        public void Configure()
        {
            ProcessAsync().Wait();
        }

        private async Task ProcessAsync()
        {
            await GameResourcesRepository.Create(
                PredefinedResourcesKeys.Gold, 
                "Gold", 
                "https://heroes.thelazy.net//images/9/9f/Gold_%28leather%29.gif", 
                1);

            var dict = YamlConfigParser<Dictionary<string, MutableGameResourceEntity>>
                .Parse("Configs/resources.yaml");

            await GameResourcesRepository.Create(dict.Select(x =>
            {
                var resourceEntity = x.Value;
                resourceEntity.Name = x.Key;
                resourceEntity.Key = x.Key;
                return resourceEntity;
            }));

            await DefaultGameResourcesRegistry.Load(CancellationToken.None);
        }
        
        public void Dispose()
        {
        }
    }
}