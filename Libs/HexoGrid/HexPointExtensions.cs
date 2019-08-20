using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace HexoGrid
{
    public static class HexPointExtensions
    {
		[NotNull]
        public static IEnumerable<HexPoint> GetAroundPoints(this HexPoint point, HexGridType gridType)
		{
		    var offsets = point.GetOffsets(gridType);
            foreach (var offset in offsets)
			{
				yield return point + offset;
			}
		}

        [NotNull]
        public static HexPoint[] GetOffsets(this HexPoint point, HexGridType gridType)
        {
            var offsetsAggByParity = gridType.GetOffsetsAggByParity();
            var pointParity = point.Parity(gridType);
            return offsetsAggByParity[pointParity];
        }

        public static int Parity(this HexPoint point, HexGridType gridType)
        {
            switch (gridType)
            {
                case HexGridType.HorizontalOdd:
                    return point.R & 1;
                case HexGridType.HorizontalEven:
                    return point.R & 1;
                case HexGridType.VerticalOdd:
                    return point.Q & 1;
                case HexGridType.VerticalEven:
                    return point.Q & 1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gridType), gridType, null);
            }
        }

	    [NotNull]
		public static IEnumerable<HexPoint> GetPointsInRange(this HexPoint point, HexGridType gridType, int range)
        {
            var cubicCenterPoint = point.ToCubeAxis(gridType);
            for (int dx = -range; dx <= range; dx++)
            {
                var max = Math.Max(-range, -dx - range);
                var min = Math.Min(range, -dx + range);
                for (int dy = max; dy <= min; dy++)
                {
                    var dz = -dx - dy;
                    var toPoint = cubicCenterPoint + new CubicHexPoint(dx, dy, dz);
                    yield return toPoint.ToOffsetsAxis(gridType);
                }
            }
        }

        public static int GetDistance(this HexPoint point1, HexPoint point2, HexGridType gridType)
        {
            var cubicPoint1 = point1.ToCubeAxis(gridType);
            var cubicPoint2 = point2.ToCubeAxis(gridType);
            return cubicPoint1.GetDistance(cubicPoint2);
        }

        public static CubicHexPoint ToCubeAxis(this HexPoint point, HexGridType gridType)
        {
            switch (gridType)
            {
                case HexGridType.HorizontalOdd:
                {
                    var col = point.Q;
                    var row = point.R;

                    var x = col - (row - (row & 1)) / 2;
                    var z = row;
                    var y = -x - z;

                    return new CubicHexPoint(x, y, z);
                }
                case HexGridType.HorizontalEven:
                {
                    var col = point.Q;
                    var row = point.R;

                    var x = col - (row + (row & 1)) / 2;
                    var z = row;
                    var y = -x - z;

                    return new CubicHexPoint(x, y, z);
                }
                case HexGridType.VerticalOdd:
                {
                    var col = point.Q;
                    var row = point.R;

                    var x = col;
                    var z = row - (col - (col & 1)) / 2;
                    var y = -x - z;

                    return new CubicHexPoint(x, y, z);
                }
                case HexGridType.VerticalEven:
                {
                    var col = point.Q;
                    var row = point.R;

                    var x = col;
                    var z = row - (col + (col & 1)) / 2;
                    var y = -x - z;

                    return new CubicHexPoint(x, y, z);
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(gridType), gridType, null);
            }
        }
	}
}
