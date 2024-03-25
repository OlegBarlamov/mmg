using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace MonoGameExtensions.DataStructures
{
    public class Array2DBasedGrid<TCell> : IBoundedGrid<Point, TCell> where TCell : IGridCell<Point>
    {
        public TCell[,] Data { get; }
        
        public int Width { get; }
        public int Height { get; }
        
        public Array2DBasedGrid([NotNull] TCell[,] data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));

            Width = data.GetLength(0);
            Height = data.GetLength(1);
        }
        
        public TCell GetCell(Point point)
        {
            return Data[point.X, point.Y];
        }

        public void SetCell(Point point, TCell data)
        {
            Data[point.X, point.Y] = data;
        }

        public Point GetLowerBound()
        {
            return Point.Zero;
        }

        public Point GetUpperBound()
        {
            return new Point(Width - 1, Height - 1);
        }

        public bool ContainsPoint(Point point)
        {
            return point.X >= GetLowerBound().X && point.X <= GetUpperBound().X &&
                   point.Y >= GetLowerBound().Y && point.Y <= GetUpperBound().Y;
        }
    }
}