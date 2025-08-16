using System.Collections.Generic;
using System.Linq;
using Epic.Core.Services.Battles;

namespace Epic.Logic.Battle.Map
{
    public static class MapUtils
    {
        public static bool IsEnemyInRange(IBattleUnitObject actor, int range, IReadOnlyCollection<IBattleUnitObject> units)
        {
            return units.Any(u => u.PlayerIndex != actor.PlayerIndex &&
                                  u.GlobalUnit.IsAlive && OddRHexoGrid.Distance(actor, u) <= range);
        }
    }
}