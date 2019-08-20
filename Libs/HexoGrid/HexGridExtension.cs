using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace HexoGrid
{
    public static class HexGridExtension
    {
        public static bool Contains<T>([NotNull] this HexGrid<T> hexGrid, HexPoint hexPoint)
        {
            if (hexGrid == null) throw new ArgumentNullException(nameof(hexGrid));
            return hexGrid.Contains(hexPoint.Q, hexPoint.R);
        }

        public static bool Contains<T>([NotNull] this HexGrid<T> hexGrid, int q, int r)
        {
            if (hexGrid == null) throw new ArgumentNullException(nameof(hexGrid));
            return q >= 0 && r >= 0 && q < hexGrid.Width && r < hexGrid.Height;
        }

        [NotNull]
        public static IEnumerable<HexPoint<T>> GetAroundPoints<T>([NotNull] this HexGrid<T> hexGrid, HexPoint point)
        {
            if (hexGrid == null) throw new ArgumentNullException(nameof(hexGrid));
            var points = point.GetAroundPoints(hexGrid.GridType);
            foreach (var p in points)
            {
                if (hexGrid.Contains(p))
                    yield return hexGrid[p];
            }
        }

	    [NotNull]
	    public static IEnumerable<HexPoint<T>> GetAroundPoints<T>([NotNull] this HexGrid<T> hexGrid, HexPoint<T> point)
	    {
		    if (hexGrid == null) throw new ArgumentNullException(nameof(hexGrid));
		    var p = point.Point;
		    return hexGrid.GetAroundPoints(p);
	    }

		[NotNull]
        public static HexPoint[] GetOffsetsForPoint<T>([NotNull] this HexGrid<T> hexGrid, HexPoint point)
        {
            if (hexGrid == null) throw new ArgumentNullException(nameof(hexGrid));
            return point.GetOffsets(hexGrid.GridType);
        }

	    [NotNull]
	    public static HexPoint[] GetOffsetsForPoint<T>([NotNull] this HexGrid<T> hexGrid, HexPoint<T> point)
	    {
		    if (hexGrid == null) throw new ArgumentNullException(nameof(hexGrid));
		    var p = point.Point;
		    return hexGrid.GetOffsetsForPoint(p);
	    }

		public static int GetDistance<T>([NotNull] this HexGrid<T> hexGrid, HexPoint point1, HexPoint point2)
        {
            if (hexGrid == null) throw new ArgumentNullException(nameof(hexGrid));
            return point1.GetDistance(point2, hexGrid.GridType);
        }

	    public static int GetDistance<T>([NotNull] this HexGrid<T> hexGrid, HexPoint<T> point1, HexPoint<T> point2)
	    {
		    if (hexGrid == null) throw new ArgumentNullException(nameof(hexGrid));
		    var p1 = point1.Point;
		    var p2 = point2.Point;
		    return hexGrid.GetDistance(p1, p2);
	    }

		[NotNull]
        public static IEnumerable<HexPoint<T>> GetPointsInRange<T>([NotNull] this HexGrid<T> grid, HexPoint point, int range)
        {
            if (grid == null) throw new ArgumentNullException(nameof(grid));
            var points = point.GetPointsInRange(grid.GridType, range);
	        return points.Select(p => grid[p]);
        }
    }
}
