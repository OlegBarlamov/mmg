using System;
using System.Threading.Tasks;
using Epic.Data.GameResources;

namespace Epic.Core.Services.GameResources
{
    public interface IGameResourcesService
    {
        Task<ResourceAmount> CreateResourceAmount(Guid resourceId, int amount);
    }
}