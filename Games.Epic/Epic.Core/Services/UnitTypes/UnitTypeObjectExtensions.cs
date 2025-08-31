using System.Collections.Generic;
using System.Linq;
using Epic.Data.GameResources;

namespace Epic.Core.Services.UnitTypes
{
    public static class UnitTypeObjectExtensions
    {
        public static IReadOnlyDictionary<string, int> GetNormalizedResourcesDistribution(this IUnitTypeObject unitType)
        {
            var copy = new Dictionary<string, int>(unitType.ResourcesDistribution);
            if (copy.Count == 0)
            {
                copy.Add(PredefinedResourcesKeys.Gold, 1);
            }
            return copy;
        }
        
        public static bool IsUpgradeFor(this IUnitTypeObject target, IUnitTypeObject unitType)
        {
            return target.UpgradeForUnitTypeIds.Contains(unitType.Id);
        }
    }
}