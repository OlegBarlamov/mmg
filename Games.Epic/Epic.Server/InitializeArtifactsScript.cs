using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Epic.Core.Services.ArtifactTypes;
using Epic.Core.Services.BuffTypes;
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
            public int Value { get; set; }
            public string ThumbnailUrl { get; set; }
            public ArtifactSlot[] Slots { get; set; } = Array.Empty<ArtifactSlot>();
            public int AttackBonus { get; set; }
            public int DefenseBonus { get; set; }
            
            [UsedImplicitly]
            public string[] Buffs { get; set; } = Array.Empty<string>();
        }

        [UsedImplicitly]
        public Dictionary<string, ArtifactTypeDeclaration> Artifacts { get; set; }
    }

    [UsedImplicitly]
    public class InitializeArtifactsScript : IAppComponent
    {
        public IArtifactTypesRepository ArtifactTypesRepository { get; }
        public DefaultArtifactTypesRegistry ArtifactTypesRegistry { get; }
        public IBuffTypesRegistry BuffTypesRegistry { get; }

        public InitializeArtifactsScript(
            [NotNull] IArtifactTypesRepository artifactTypesRepository,
            [NotNull] DefaultArtifactTypesRegistry artifactTypesRegistry,
            [NotNull] IBuffTypesRegistry buffTypesRegistry)
        {
            ArtifactTypesRepository = artifactTypesRepository ?? throw new ArgumentNullException(nameof(artifactTypesRepository));
            ArtifactTypesRegistry = artifactTypesRegistry ?? throw new ArgumentNullException(nameof(artifactTypesRegistry));
            BuffTypesRegistry = buffTypesRegistry ?? throw new ArgumentNullException(nameof(buffTypesRegistry));
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

                var fieldsObj = new ArtifactTypeEntityFields
                {
                    Key = key,
                    Name = x.Name,
                    Value = x.Value,
                    ThumbnailUrl = x.ThumbnailUrl,
                    Slots = x.Slots?.ToArray() ?? Array.Empty<ArtifactSlot>(),
                    AttackBonus = x.AttackBonus,
                    DefenseBonus = x.DefenseBonus,
                };
                
                // Resolve BuffTypes string keys to GUIDs
                if (x.Buffs != null && x.Buffs.Length > 0)
                {
                    fieldsObj.BuffTypeIds = x.Buffs
                        .Where(buffKey => !string.IsNullOrWhiteSpace(buffKey))
                        .Select(buffKey => BuffTypesRegistry.ByKey(buffKey).Id)
                        .ToArray();
                }
                
                return fieldsObj;
            }).ToArray<IArtifactTypeEntityFields>();

            await ArtifactTypesRepository.CreateBatch(fields);
            await ArtifactTypesRegistry.Load(CancellationToken.None);
        }

        public void Dispose()
        {
        }
    }
}

