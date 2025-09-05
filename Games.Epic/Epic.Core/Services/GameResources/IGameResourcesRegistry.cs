using System.Collections.Generic;
using Epic.Data.GameResources;

namespace Epic.Core.Services.GameResources
{
    public interface IGameResourcesRegistry
    {
        IReadOnlyCollection<IGameResourceEntity> GetAll();
    }
}