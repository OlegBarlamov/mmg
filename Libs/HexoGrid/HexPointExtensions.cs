using System.Collections.Generic;
using JetBrains.Annotations;

namespace HexoGrid
{
    public static class HexPointExtensions
    {
		[NotNull, ItemNotNull]
        public static IEnumerable<HexPoint> GetAroundPoints(this HexPoint point, bool includeSelf = false)
		{
			var parity = point.Y;
			var dir = Directions[parity]
		}

	    private static readonly HexPoint[][] Directions = 
		{
		    new[]
		    {
			    new HexPoint(1,0), new HexPoint(0,-1), new HexPoint(-1,-1),
				new HexPoint(-1,0), new HexPoint(-1,1), new HexPoint(0,1),
			},
		    new[]
		    {
			    new HexPoint(1,0), new HexPoint(1,-1), new HexPoint(0,-1),
			    new HexPoint(-1,0), new HexPoint(0,1), new HexPoint(1,1),
		    }
		};
	}
}
