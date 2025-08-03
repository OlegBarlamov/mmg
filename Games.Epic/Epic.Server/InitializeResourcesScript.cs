using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.GameResources;
using FrameworkSDK;
using JetBrains.Annotations;
using YamlDotNet.Serialization;

namespace Epic.Server
{
    [UsedImplicitly]
    public class InitializeResourcesScript : IAppComponent
    {
        public IGameResourcesRepository GameResourcesRepository { get; }

        public InitializeResourcesScript([NotNull] IGameResourcesRepository gameResourcesRepository)
        {
            GameResourcesRepository = gameResourcesRepository ?? throw new ArgumentNullException(nameof(gameResourcesRepository));
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
            
            var yaml = await File.ReadAllTextAsync("Configs/resources.yaml");
            var deserializer = new DeserializerBuilder().Build();
            var dict = deserializer.Deserialize<Dictionary<string, MutableGameResourceEntity>>(yaml);

            await GameResourcesRepository.Create(dict.Select(x =>
            {
                var resourceEntity = x.Value;
                resourceEntity.Name = x.Key;
                resourceEntity.Key = x.Key;
                return resourceEntity;
            }));
        }
        
        public void Dispose()
        {
        }
    }
}