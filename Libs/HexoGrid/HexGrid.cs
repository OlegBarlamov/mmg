using System;
using JetBrains.Annotations;

namespace HexoGrid
{
    public class HexGrid<T>
    {
	    public int Width { get; }
	    public int Height { get; }
	    public HexGridType GridType { get; }

	    public HexPoint<T> this[int q, int r] => _points[q, r];
	    public HexPoint<T> this[HexPoint point] => _points[point.Q, point.R];

	    private readonly HexPoint<T>[,] _points;

        public HexGrid(int width, int height, HexGridType gridType, [NotNull] Func<int, int, T> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            Width = width;
            Height = height;
            GridType = gridType;

            _points = new HexPoint<T>[Width, Height];
            for (int q = 0; q < Width; q++)
            {
                for (int r = 0; r < Height; r++)
                {
                    var data = factory(q, r);
                    _points[q, r] = new HexPoint<T>(q, r, this, data);
                }
            }
        }

        public HexGrid(int width, int height, HexGridType gridType, [NotNull] Func<T> factory)
            : this(width, height, gridType, (q, r) => factory())
        {
        }

        public HexGrid(int width, int height, HexGridType gridType)
            : this(width, height, gridType, () => default(T))
	    {
		}


        public override string ToString()
	    {
		    return $"{GridType}<{nameof(T)}>:{Width}x{Height}";
	    }
    }
}
