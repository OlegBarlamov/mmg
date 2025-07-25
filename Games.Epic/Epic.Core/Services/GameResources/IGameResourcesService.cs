using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epic.Data.GameResources;

namespace Epic.Core.Services.GameResources
{
    public interface IGameResourcesService
    {
        Task<ResourceAmount> CreateResourceAmount(Guid resourceId, int amount);
        Task<ResourceAmount[]> GetResourcesAmountsFromPrice(Price price);
        Task<IReadOnlyList<ResourceAmount[]>> GetResourcesAmountsFromPrices(IReadOnlyList<Price> prices);
    }
}