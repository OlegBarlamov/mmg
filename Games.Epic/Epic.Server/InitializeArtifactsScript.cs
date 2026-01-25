using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.ArtifactType;
using Epic.Data.Artifacts;
using FrameworkSDK;
using JetBrains.Annotations;

namespace Epic.Server
{
    internal class ArtifactsConfig
    {
        [UsedImplicitly]
        internal class ArtifactTypeDeclaration
        {
            public string Key { get; set; }
            public string Name { get; set; }
            public string ThumbnailUrl { get; set; }
            public ArtifactSlot[] Slots { get; set; } = Array.Empty<ArtifactSlot>();
            public int AttackBonus { get; set; }
            public int DefenseBonus { get; set; }
        }

        [UsedImplicitly]
        public Dictionary<string, ArtifactTypeDeclaration> Artifacts { get; set; }
    }

    [UsedImplicitly]
    public class InitializeArtifactsScript : IAppComponent
    {
        public IArtifactTypesRepository ArtifactTypesRepository { get; }

        public InitializeArtifactsScript([NotNull] IArtifactTypesRepository artifactTypesRepository)
        {
            ArtifactTypesRepository = artifactTypesRepository ?? throw new ArgumentNullException(nameof(artifactTypesRepository));
        }

        public void Configure()
        {
            ProcessAsync().Wait();
        }

        private async Task ProcessAsync()
        {
            var config = YamlConfigParser<ArtifactsConfig>.Parse("Configs/artifacts.yaml");
            if (config?.Artifacts == null || config.Artifacts.Count == 0)
                return;

            var fields = config.Artifacts.Select(kv =>
            {
                var x = kv.Value ?? new ArtifactsConfig.ArtifactTypeDeclaration();

                // Keep keys stable: if YAML doesn't specify Key, use dictionary key.
                var key = string.IsNullOrWhiteSpace(x.Key) ? kv.Key : x.Key;

                return new ArtifactTypeEntityFields
                {
                    Key = key,
                    Name = x.Name,
                    ThumbnailUrl = x.ThumbnailUrl,
                    Slots = x.Slots?.ToArray() ?? Array.Empty<ArtifactSlot>(),
                    AttackBonus = x.AttackBonus,
                    DefenseBonus = x.DefenseBonus,
                };
            }).ToArray<IArtifactTypeEntityFields>();

            await ArtifactTypesRepository.CreateBatch(fields);
        }

        public void Dispose()
        {
        }
    }
}

