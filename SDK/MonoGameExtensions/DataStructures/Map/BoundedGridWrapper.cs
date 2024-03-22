using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace MonoGameExtensions.DataStructures
{
    public class BoundedGridWrapper<TPoint, TCell> : IBoundedGrid<TPoint, TCell> where TCell : IGridCell<TPoint> where TPoint : struct
    {
        public TPoint LowerBound { get; }
        public TPoint UpperBound { get; }

        [NotNull] private IGrid<TPoint, TCell> Grid { get; }
        
        private Func<IBoundedGrid<TPoint, TCell>, TPoint, bool> ContainsPointFunction { get; }

        public BoundedGridWrapper([NotNull] IGrid<TPoint, TCell> grid, TPoint lowerBound, TPoint upperBound,
            [NotNull] Func<IBoundedGrid<TPoint, TCell>, TPoint, bool> containsPointFunction)
        {
            Grid = grid ?? throw new ArgumentNullException(nameof(grid));
            LowerBound = lowerBound;
            UpperBound = upperBound;
            ContainsPointFunction = containsPointFunction ?? throw new ArgumentNullException(nameof(containsPointFunction));
        }

        public TCell GetCell(TPoint point)
        {
            if (!ContainsPoint(point))
                throw new ArgumentOutOfRangeException(nameof(point), point, "Point is out of range");

            return Grid.GetCell(point);
        }

        public void SetCell(TPoint point, TCell data)
        {
            if (!ContainsPoint(point))
                throw new ArgumentOutOfRangeException(nameof(point), point, "Point is out of range");
            
            Grid.SetCell(point, data);
        }

        public TPoint GetLowerBound()
        {
            return LowerBound;
        }

        public TPoint GetUpperBound()
        {
            return UpperBound;
        }

        public bool ContainsPoint(TPoint point)
        {
            return ContainsPointFunction.Invoke(this, point);
        }
    }
}