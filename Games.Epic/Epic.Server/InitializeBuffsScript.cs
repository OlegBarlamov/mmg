using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Epic.Core.Services.BuffTypes;
using Epic.Data.BuffType;
using FrameworkSDK;
using JetBrains.Annotations;

namespace Epic.Server
{
    internal class BuffsConfig
    {
        [UsedImplicitly]
        internal class BuffTypeDeclaration : BuffFields
        {
        }

        [UsedImplicitly]
        public Dictionary<string, BuffTypeDeclaration> Buffs { get; set; }
    }

    [UsedImplicitly]
    public class InitializeBuffsScript : IAppComponent
    {
        public IBuffTypesRepository BuffTypesRepository { get; }
        public DefaultBuffTypesRegistry BuffTypesRegistry { get; }

        public InitializeBuffsScript(
            [NotNull] IBuffTypesRepository buffTypesRepository,
            [NotNull] DefaultBuffTypesRegistry buffTypesRegistry)
        {
            BuffTypesRepository = buffTypesRepository ?? throw new ArgumentNullException(nameof(buffTypesRepository));
            BuffTypesRegistry = buffTypesRegistry ?? throw new ArgumentNullException(nameof(buffTypesRegistry));
        }

        public void Configure()
        {
            ProcessAsync().Wait();
        }

        private async Task ProcessAsync()
        {
            var config = YamlConfigParser<BuffsConfig>
                .Parse("Configs/buffs.yaml");

            if (config?.Buffs == null || config.Buffs.Count == 0)
                return;

            var fields = config.Buffs.Select(kv =>
            {
                var x = kv.Value ?? new BuffsConfig.BuffTypeDeclaration();
                var key = string.IsNullOrWhiteSpace(x.Key) ? kv.Key : x.Key;
                x.Key = key;
                if (string.IsNullOrWhiteSpace(x.Name))
                    x.Name = key;
                return (IBuffFields)x;
            }).ToArray();

            await BuffTypesRepository.CreateBatch(fields);
            await BuffTypesRegistry.Load(CancellationToken.None);
        }

        public void Dispose()
        {
        }
    }
}

