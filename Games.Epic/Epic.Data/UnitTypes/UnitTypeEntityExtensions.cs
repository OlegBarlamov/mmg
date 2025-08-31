using System.Linq;

namespace Epic.Data.UnitTypes
{
    public static class UnitTypeEntityExtensions
    {
        public static bool IsUpgradeFor(this IUnitTypeEntity target, IUnitTypeEntity unitType)
        {
            return target.UpgradeForUnitTypeIds.Contains(unitType.Id);
        }
    }
}