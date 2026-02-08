using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Epic.Core.Services.EffectTypes;
using Epic.Data.EffectType;
using FrameworkSDK;
using JetBrains.Annotations;

namespace Epic.Server
{
    internal class EffectsConfig
    {
        [UsedImplicitly]
        internal class EffectTypeDeclaration : EffectTypeFields
        {
        }

        [UsedImplicitly]
        public Dictionary<string, EffectTypeDeclaration> EffectTypes { get; set; }
    }

    [UsedImplicitly]
    public class InitializeEffectsScript : IAppComponent
    {
        public IEffectTypesRepository EffectTypesRepository { get; }
        public DefaultEffectTypesRegistry EffectTypesRegistry { get; }

        public InitializeEffectsScript(
            [NotNull] IEffectTypesRepository effectTypesRepository,
            [NotNull] DefaultEffectTypesRegistry effectTypesRegistry)
        {
            EffectTypesRepository = effectTypesRepository ?? throw new ArgumentNullException(nameof(effectTypesRepository));
            EffectTypesRegistry = effectTypesRegistry ?? throw new ArgumentNullException(nameof(effectTypesRegistry));
        }

        public void Configure()
        {
            ProcessAsync().GetAwaiter().GetResult();
        }

        private async Task ProcessAsync()
        {
            var config = YamlConfigParser<EffectsConfig>
                .Parse("Configs/effects.yaml");

            if (config?.EffectTypes == null || config.EffectTypes.Count == 0)
                return;

            var fields = config.EffectTypes.Select(kv =>
            {
                var x = kv.Value ?? new EffectsConfig.EffectTypeDeclaration();
                var key = kv.Key;
                x.Key = key;
                if (string.IsNullOrWhiteSpace(x.Name))
                    x.Name = key;
                return (IEffectTypeFields)x;
            }).ToArray();

            await EffectTypesRepository.CreateBatch(fields);
            await EffectTypesRegistry.Load(CancellationToken.None);
        }

        public void Dispose()
        {
        }
    }
}
