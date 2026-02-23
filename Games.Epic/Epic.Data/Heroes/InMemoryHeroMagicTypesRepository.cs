using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Epic.Data.Heroes
{
    [UsedImplicitly]
    public class InMemoryHeroMagicTypesRepository : IHeroMagicTypesRepository
    {
        public string Name => nameof(InMemoryHeroMagicTypesRepository);
        public string EntityName => "HeroMagicType";

        private readonly List<(Guid HeroId, Guid MagicTypeId)> _relations = new List<(Guid, Guid)>();

        public Task<Guid[]> GetMagicTypeIdsByHeroId(Guid heroId)
        {
            var ids = _relations
                .Where(r => r.HeroId == heroId)
                .Select(r => r.MagicTypeId)
                .Distinct()
                .ToArray();
            return Task.FromResult(ids);
        }

        public Task Add(Guid heroId, Guid magicTypeId)
        {
            if (heroId == Guid.Empty || magicTypeId == Guid.Empty)
                return Task.CompletedTask;
            var exists = _relations.Any(r => r.HeroId == heroId && r.MagicTypeId == magicTypeId);
            if (!exists)
                _relations.Add((heroId, magicTypeId));
            return Task.CompletedTask;
        }
    }
}
