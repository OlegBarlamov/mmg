using System.Collections.Generic;
using JetBrains.Annotations;

namespace HexoGrid
{
    public static class HexPointExtensions
    {
        [NotNull, ItemNotNull]
        public static IEnumerable<HexPoint> GetAroundPoints(this HexPoint point, bool includeSelf = false)
        {

        }
    }
}
