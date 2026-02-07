using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Epic.Data.EffectType;
using JetBrains.Annotations;

namespace Epic.Core.Services.EffectTypes
{
    [UsedImplicitly]
    public class DefaultEffectTypesRegistry : IEffectTypesRegistry
    {
        public IReadOnlyList<IEffectTypeEntity> All => _all;

        private IEffectTypesRepository Repository { get; }

        private readonly List<IEffectTypeEntity> _all = new List<IEffectTypeEntity>();
        private readonly Dictionary<Guid, IEffectTypeEntity> _byId = new Dictionary<Guid, IEffectTypeEntity>();
        private readonly Dictionary<string, IEffectTypeEntity> _byKey = new Dictionary<string, IEffectTypeEntity>();

        public DefaultEffectTypesRegistry([NotNull] IEffectTypesRepository repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IEffectTypeEntity ById(Guid id) => _byId[id];

        public IEffectTypeEntity ByKey(string key) => _byKey[key];

        public async Task Load(CancellationToken cancellationToken)
        {
            _all.Clear();
            _byId.Clear();
            _byKey.Clear();

            var all = await Repository.GetAll();
            _all.AddRange(all);

            foreach (var x in all)
            {
                _byId[x.Id] = x;
                if (!string.IsNullOrWhiteSpace(x.Key))
                    _byKey[x.Key] = x;
            }
        }
    }
}
