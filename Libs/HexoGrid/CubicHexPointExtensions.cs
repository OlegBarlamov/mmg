using System;


namespace HexoGrid
{
    public static class CubicHexPointExtensions
    {
        public static int GetDistance(this CubicHexPoint point1, CubicHexPoint point2)
        {
            return (Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y) + Math.Abs(point1.Z - point2.Z)) / 2;
        }

        public static HexPoint ToOffsetsAxis(this CubicHexPoint point, HexGridType gridType)
        {
            switch (gridType)
            {
                case HexGridType.HorizontalOdd:
                {
                    var x = point.X;
                    var y = point.Y;
                    var z = point.Z;

                    var col = x + (z - (z & 1)) / 2;
                    var row = z;

                    return new HexPoint(col, row);
                }
                case HexGridType.HorizontalEven:
                {
                    var x = point.X;
                    var y = point.Y;
                    var z = point.Z;

                    var col = x + (z + (z & 1)) / 2;
                    var row = z;

                    return new HexPoint(col, row);
                }
                case HexGridType.VerticalOdd:
                {
                    var x = point.X;
                    var y = point.Y;
                    var z = point.Z;

                    var col = x;
                    var row = z + (x - (x & 1)) / 2;

                    return new HexPoint(col, row);
                }
                case HexGridType.VerticalEven:
                {
                    var x = point.X;
                    var y = point.Y;
                    var z = point.Z;

                    var col = x;
                    var row = z + (x + (x & 1)) / 2;

                    return new HexPoint(col, row);
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(gridType), gridType, null);
            }
        }
    }
}
