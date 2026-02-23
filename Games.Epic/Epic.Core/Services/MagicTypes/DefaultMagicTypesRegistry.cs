using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Epic.Data.MagicType;
using JetBrains.Annotations;

namespace Epic.Core.Services.MagicTypes
{
    [UsedImplicitly]
    public class DefaultMagicTypesRegistry : IMagicTypesRegistry
    {
        public IReadOnlyList<IMagicTypeEntity> All => _all;

        private IMagicTypesRepository Repository { get; }

        private readonly List<IMagicTypeEntity> _all = new List<IMagicTypeEntity>();
        private readonly Dictionary<Guid, IMagicTypeEntity> _byId = new Dictionary<Guid, IMagicTypeEntity>();
        private readonly Dictionary<string, IMagicTypeEntity> _byKey = new Dictionary<string, IMagicTypeEntity>();

        public DefaultMagicTypesRegistry([NotNull] IMagicTypesRepository repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IMagicTypeEntity ById(Guid id) => _byId[id];

        public IMagicTypeEntity ByKey(string key) => _byKey[key];

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
