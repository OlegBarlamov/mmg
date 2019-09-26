using HexoGrid;

namespace Epic.Data.Battle
{
    public class FieldCell
    {
        public HexPoint Point { get; }
        public FieldTile Tile { get; }

        public FieldCell(HexPoint point, FieldTile tile)
        {
            Point = point;
            Tile = tile;
        }
    }
}
