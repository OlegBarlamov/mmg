using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Epic.Data.ArtifactType;
using JetBrains.Annotations;

namespace Epic.Core.Services.ArtifactTypes
{
    [UsedImplicitly]
    public class DefaultArtifactTypesRegistry : IArtifactTypesRegistry
    {
        public IReadOnlyList<IArtifactTypeEntity> AllOrderedByValue => _orderedArtifactTypes;

        private IArtifactTypesRepository Repository { get; }
        private readonly List<IArtifactTypeEntity> _orderedArtifactTypes = new List<IArtifactTypeEntity>();
        private readonly Dictionary<Guid, IArtifactTypeEntity> _artifactTypesById = new Dictionary<Guid, IArtifactTypeEntity>();
        private readonly Dictionary<string, IArtifactTypeEntity> _artifactTypesByKey = new Dictionary<string, IArtifactTypeEntity>();

        public DefaultArtifactTypesRegistry([NotNull] IArtifactTypesRepository repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IArtifactTypeEntity ById(Guid typeId) => _artifactTypesById[typeId];

        public IArtifactTypeEntity ByKey(string key) => _artifactTypesByKey[key];

        public async Task Load(CancellationToken cancellationToken)
        {
            _orderedArtifactTypes.Clear();
            _artifactTypesById.Clear();
            _artifactTypesByKey.Clear();

            var all = await Repository.GetAll();
            _orderedArtifactTypes.AddRange(all);
            _orderedArtifactTypes.Sort((x, y) => x.Value.CompareTo(y.Value));

            foreach (var x in all)
            {
                _artifactTypesById[x.Id] = x;
                if (!string.IsNullOrWhiteSpace(x.Key))
                    _artifactTypesByKey[x.Key] = x;
            }
        }
    }
}

