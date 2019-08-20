using System;
using JetBrains.Annotations;

namespace HexoGrid
{
    public class HexPoint<T>
    {
	    public int Q => Point.Q;
	    public int R => Point.R;

	    [CanBeNull] public T Data { get; set; }

		[NotNull] internal HexGrid<T> OwnedGrid { get; }

		internal HexPoint Point { get; }

		internal HexPoint(HexPoint point, [NotNull] HexGrid<T> ownedGrid, T data = default(T))
		{
			OwnedGrid = ownedGrid ?? throw new ArgumentNullException(nameof(ownedGrid));
			Point = point;
			Data = data;
		}

	    internal HexPoint(int q, int r, [NotNull] HexGrid<T> ownedGrid, T data = default(T))
			:this(new HexPoint(q,r), ownedGrid, data)
	    {
	    }
	}
}
