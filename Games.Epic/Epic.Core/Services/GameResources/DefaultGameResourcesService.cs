using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Data.GameResources;
using JetBrains.Annotations;

namespace Epic.Core.Services.GameResources
{
    [UsedImplicitly]
    public class DefaultGameResourcesService : IGameResourcesService
    {
        public IGameResourcesRepository GameResourcesRepository { get; }

        public DefaultGameResourcesService([NotNull] IGameResourcesRepository gameResourcesRepository)
        {
            GameResourcesRepository = gameResourcesRepository ?? throw new ArgumentNullException(nameof(gameResourcesRepository));
        }

        public Guid GoldResourceId => GameResourcesRepository.GoldResourceId;

        public async Task<ResourceAmount> CreateResourceAmount(Guid resourceId, int amount)
        {
            var entity = await GameResourcesRepository.GetById(resourceId);
            return ResourceAmount.Create(entity, amount);
        }

        public async Task<ResourceAmount[]> GetResourcesAmountsFromPrice(Price price)
        {
            var result = await GetResourcesAmountsFromPrices(new[] { price });
            return result[0];
        }

        public async Task<IReadOnlyList<ResourceAmount[]>> GetResourcesAmountsFromPrices(IReadOnlyList<Price> prices)
        {
            var resourcesIds = prices.SelectMany(x => x.PerResourcePrice.Keys).Distinct();
            var resourceEntities = await GameResourcesRepository.GetByIds(resourcesIds);
            var resourceEntitiesByIds = resourceEntities.ToDictionary(x => x.Id, x => x);

            return prices.Select(x => x.PerResourcePrice.Select(keyValue =>
                {
                    var id = keyValue.Key;
                    var amount = keyValue.Value;

                    return ResourceAmount.Create(resourceEntitiesByIds[id], amount);
                }).ToArray()
            ).ToArray();
        }
    }
}
