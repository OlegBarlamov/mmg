using System;
using System.Collections.Generic;
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

        public static IEnumerable<T> GetInRectangle<T>(this IGrid<Point, T> grid, Rectangle rectangle) where T : class, IGridCell<Point>
        {
            for (int x = rectangle.Left; x < rectangle.Right; x++)
            {
                for (int y = rectangle.Top; y < rectangle.Bottom; y++)
                {
                    var cell = grid.GetCell(new Point(x, y));
                    if (cell != null)
                        yield return cell;
                }
            }
        }
        
        public static IEnumerable<T> GetInRectangleBounded<T>(this IBoundedGrid<Point, T> grid, Rectangle rectangle) where T : class, IGridCell<Point>
        {
            var lower = grid.GetLowerBound();
            var upper = grid.GetUpperBound();
            var left = Math.Max(rectangle.Left, lower.X);
            var right = Math.Min(rectangle.Right, upper.X);
            var top = Math.Max(rectangle.Top, lower.Y);
            var bottom = Math.Min(rectangle.Bottom, upper.Y);
            
            for (int x = left; x < right; x++)
            {
                for (int y = top; y < bottom; y++)
                {
                    var cell = grid.GetCell(new Point(x, y));
                    if (cell != null)
                        yield return cell;
                }
            }
        }

        public static T GetCell<T>(this IGrid<Point, T> grid, int x, int y)
            where T : class, IGridCell<Point>
        {
            return grid.GetCell(new Point(x, y));
        }

        public static bool ContainsPoint<T>(this IBoundedGrid<Point, T> grid, Point point)
            where T : class, IGridCell<Point>
        {
            return point.InRange(grid.GetLowerBound(), grid.GetUpperBound());
        }
    }
}