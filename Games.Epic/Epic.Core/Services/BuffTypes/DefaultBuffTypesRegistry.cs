using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Epic.Data.BuffType;
using JetBrains.Annotations;

namespace Epic.Core.Services.BuffTypes
{
    [UsedImplicitly]
    public class DefaultBuffTypesRegistry : IBuffTypesRegistry
    {
        public IReadOnlyList<IBuffTypeEntity> All => _all;

        private IBuffTypesRepository Repository { get; }

        private readonly List<IBuffTypeEntity> _all = new List<IBuffTypeEntity>();
        private readonly Dictionary<Guid, IBuffTypeEntity> _byId = new Dictionary<Guid, IBuffTypeEntity>();
        private readonly Dictionary<string, IBuffTypeEntity> _byKey = new Dictionary<string, IBuffTypeEntity>();

        public DefaultBuffTypesRegistry([NotNull] IBuffTypesRepository repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IBuffTypeEntity ById(Guid id) => _byId[id];

        public IBuffTypeEntity ByKey(string key) => _byKey[key];

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

