using Microsoft.Xna.Framework;

namespace MonoGameExtensions.DataStructures
{
    public static class GridExtensions
    {
        public static FlatAdjusting<T> GetAdjusting<T>(this IGrid<Point, T> grid, Point center) where T : IGridCell<Point>
        {
            return new FlatAdjusting<T>
            {
                Left = grid.GetCell(center.GetLeft()),
                Bottom = grid.GetCell(center.GetBottom()),
                Center = grid.GetCell(center),
                LeftBottom = grid.GetCell(center.GetLeftBottom()),
                LeftTop = grid.GetCell(center.GetLeftTop()),
                Right = grid.GetCell(center.GetRight()),
                RightBottom = grid.GetCell(center.GetRightBottom()),
                RightTop = grid.GetCell(center.GetRightTop()),
                Top = grid.GetCell(center.GetTop()),
            };
        }
    }
}