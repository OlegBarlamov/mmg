using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace HexoGrid
{
    public static class HexPointTemplatedExtensions
    {
	    public static HexPoint Point<T>([NotNull] this HexPoint<T> point)
	    {
		    if (point == null) throw new ArgumentNullException(nameof(point));
		    return point.Point;
	    }

	    [NotNull]
	    public static IEnumerable<HexPoint<T>> GetAroundPoints<T>([NotNull] this HexPoint<T> point)
	    {
		    if (point == null) throw new ArgumentNullException(nameof(point));
		    return point.OwnedGrid.GetAroundPoints(point);
	    }

	    [NotNull]
	    public static HexPoint[] GetOffsets<T>([NotNull] this HexPoint<T> point)
	    {
		    if (point == null) throw new ArgumentNullException(nameof(point));
		    return point.OwnedGrid.GetOffsetsForPoint(point);
	    }

	    public static int Parity<T>([NotNull] this HexPoint<T> point)
	    {
		    if (point == null) throw new ArgumentNullException(nameof(point));
		    return point.Point.Parity(point.OwnedGrid.GridType);
	    }

	    [NotNull]
		public static IEnumerable<HexPoint<T>> GetPointsInRange<T>([NotNull] this HexPoint<T> point, int range)
	    {
		    if (point == null) throw new ArgumentNullException(nameof(point));
		    if (range < 0) throw new ArgumentOutOfRangeException(nameof(range));
		    return point.OwnedGrid.GetPointsInRange(point.Point, range);

	    }

	    public static int GetDistance<T>([NotNull] this HexPoint<T> point1, [NotNull] HexPoint<T> point2)
	    {
		    if (point1 == null) throw new ArgumentNullException(nameof(point1));
		    if (point2 == null) throw new ArgumentNullException(nameof(point2));
		    if (point1.OwnedGrid != point2.OwnedGrid)
				throw new ArgumentException($"{nameof(point1.OwnedGrid)}!={nameof(point2.OwnedGrid)}");

		    var p1 = point1.Point;
		    var p2 = point2.Point;
		    return p1.GetDistance(p2, point1.OwnedGrid.GridType);
	    }
	}
}
