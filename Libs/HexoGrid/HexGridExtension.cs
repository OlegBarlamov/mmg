using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace HexoGrid
{
    public static class HexGridExtension
    {
        public static bool Contains([NotNull] this HexGrid hexGrid, HexPoint hexPoint)
        {
            if (hexGrid == null) throw new ArgumentNullException(nameof(hexGrid));
            return hexGrid.Contains(hexPoint.Q, hexPoint.R);
        }

        public static bool Contains([NotNull] this HexGrid hexGrid, int q, int r)
        {
            if (hexGrid == null) throw new ArgumentNullException(nameof(hexGrid));
            return q >= 0 && r >= 0 && q < hexGrid.Width && r < hexGrid.Height;
        }

        [NotNull]
        public static IEnumerable<HexPoint> GetAroundPoints([NotNull] this HexGrid hexGrid, HexPoint point)
        {
            if (hexGrid == null) throw new ArgumentNullException(nameof(hexGrid));
            var points = point.GetAroundPoints(hexGrid.GridType);
            foreach (var p in points)
            {
                if (hexGrid.Contains(p))
                    yield return p;
            }
        }

        [NotNull]
        public static HexPoint[] GetOffsetsForPoint([NotNull] this HexGrid hexGrid, HexPoint point)
        {
            if (hexGrid == null) throw new ArgumentNullException(nameof(hexGrid));
            return point.GetOffsets(hexGrid.GridType);
        }

        public static int GetDistance([NotNull] this HexGrid hexGrid, HexPoint point1, HexPoint point2)
        {
            if (hexGrid == null) throw new ArgumentNullException(nameof(hexGrid));
            return point1.GetDistance(point2, hexGrid.GridType);
        }

        [NotNull]
        public static IEnumerable<HexPoint> GetPointsInRange([NotNull] this HexGrid grid, HexPoint point, int range)
        {
            if (grid == null) throw new ArgumentNullException(nameof(grid));
            return point.GetPointsInRange(grid.GridType, range);
        }
    }
}
