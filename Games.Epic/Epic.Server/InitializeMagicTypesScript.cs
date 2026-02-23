using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Epic.Core.Services.BuffTypes;
using Epic.Core.Services.EffectTypes;
using Epic.Core.Services.MagicTypes;
using Epic.Data.MagicType;
using FrameworkSDK;
using JetBrains.Annotations;

namespace Epic.Server
{
    internal class MagicTypesConfig
    {
        [UsedImplicitly]
        internal class MagicTypeDeclaration : MagicTypeFields
        {
            [UsedImplicitly]
            public List<string> ApplyBuffs { get; set; }

            [UsedImplicitly]
            public List<string> ApplyEffects { get; set; }
        }

        [UsedImplicitly]
        public Dictionary<string, MagicTypeDeclaration> MagicTypes { get; set; }
    }

    [UsedImplicitly]
    public class InitializeMagicTypesScript : IAppComponent
    {
        public IMagicTypesRepository MagicTypesRepository { get; }
        public DefaultMagicTypesRegistry MagicTypesRegistry { get; }
        public IBuffTypesRegistry BuffTypesRegistry { get; }
        public IEffectTypesRegistry EffectTypesRegistry { get; }

        public InitializeMagicTypesScript(
            [NotNull] IMagicTypesRepository magicTypesRepository,
            [NotNull] DefaultMagicTypesRegistry magicTypesRegistry,
            [NotNull] IBuffTypesRegistry buffTypesRegistry,
            [NotNull] IEffectTypesRegistry effectTypesRegistry)
        {
            MagicTypesRepository = magicTypesRepository ?? throw new ArgumentNullException(nameof(magicTypesRepository));
            MagicTypesRegistry = magicTypesRegistry ?? throw new ArgumentNullException(nameof(magicTypesRegistry));
            BuffTypesRegistry = buffTypesRegistry ?? throw new ArgumentNullException(nameof(buffTypesRegistry));
            EffectTypesRegistry = effectTypesRegistry ?? throw new ArgumentNullException(nameof(effectTypesRegistry));
        }

        public void Configure()
        {
            ProcessAsync().GetAwaiter().GetResult();
        }

        private async Task ProcessAsync()
        {
            var config = YamlConfigParser<MagicTypesConfig>
                .Parse("Configs/magic.yaml");

            if (config?.MagicTypes == null || config.MagicTypes.Count == 0)
                return;

            var fields = config.MagicTypes.Select(kv =>
            {
                var x = kv.Value ?? new MagicTypesConfig.MagicTypeDeclaration();
                var key = kv.Key;
                x.Key = key;
                if (string.IsNullOrWhiteSpace(x.Name))
                    x.Name = key;

                if (x.ApplyBuffs != null && x.ApplyBuffs.Count > 0)
                {
                    x.ApplyBuffsIds = x.ApplyBuffs
                        .Where(b => !string.IsNullOrWhiteSpace(b))
                        .Select(b => BuffTypesRegistry.ByKey(b).Id)
                        .ToArray();
                }
                else
                {
                    x.ApplyBuffsIds = Array.Empty<Guid>();
                }

                if (x.ApplyEffects != null && x.ApplyEffects.Count > 0)
                {
                    x.ApplyEffectsIds = x.ApplyEffects
                        .Where(e => !string.IsNullOrWhiteSpace(e))
                        .Select(e => EffectTypesRegistry.ByKey(e).Id)
                        .ToArray();
                }
                else
                {
                    x.ApplyEffectsIds = Array.Empty<Guid>();
                }

                return (IMagicTypeFields)x;
            }).ToArray();

            await MagicTypesRepository.CreateBatch(fields);
            await MagicTypesRegistry.Load(CancellationToken.None);
        }

        public void Dispose()
        {
        }
    }
}
