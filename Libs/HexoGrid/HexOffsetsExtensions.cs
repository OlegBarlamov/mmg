using System;

namespace HexoGrid
{
    public static class HexOffsetsExtensions
    {
        public static HexPoint[] GetAll(this HexOffsets.IHorizontalOffsets horizontalOffset)
        {
            return new[]
            {
                horizontalOffset.UpLeft,
                horizontalOffset.UpRight,
                horizontalOffset.Right,
                horizontalOffset.DownRight,
                horizontalOffset.DownLeft,
                horizontalOffset.Left
            };
        }

        public static HexPoint[] GetAll(this HexOffsets.IVerticalOffsets verticalOffset)
        {
            return new[]
            {
                verticalOffset.Up,
                verticalOffset.UpRight,
                verticalOffset.DownRight,
                verticalOffset.Down,
                verticalOffset.DownLeft,
                verticalOffset.UpLeft,
            };
        }

        public static HexPoint GetForDirection(this HexOffsets.IHorizontalOffsets horizontalOffset, HexDirections.Horizontal direction)
        {
            switch (direction)
            {
                case HexDirections.Horizontal.UpLeft:
                    return horizontalOffset.UpLeft;
                case HexDirections.Horizontal.UpRight:
                    return horizontalOffset.UpRight;
                case HexDirections.Horizontal.Right:
                    return horizontalOffset.Right;
                case HexDirections.Horizontal.DownRight:
                    return horizontalOffset.DownRight;
                case HexDirections.Horizontal.DownLeft:
                    return horizontalOffset.DownLeft;
                case HexDirections.Horizontal.Left:
                    return horizontalOffset.Left;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public static HexPoint GetForDirection(this HexOffsets.IVerticalOffsets verticalOffset, HexDirections.Vertical direction)
        {
            switch (direction)
            {
                case HexDirections.Vertical.Up:
                    return verticalOffset.Up;
                case HexDirections.Vertical.UpRight:
                    return verticalOffset.UpRight;
                case HexDirections.Vertical.DownRight:
                    return verticalOffset.DownRight;
                case HexDirections.Vertical.Down:
                    return verticalOffset.Down;
                case HexDirections.Vertical.DownLeft:
                    return verticalOffset.DownLeft;
                case HexDirections.Vertical.UpLeft:
                    return verticalOffset.UpLeft;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
    }
}