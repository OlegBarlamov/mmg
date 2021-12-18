using HexoGrid;
using NetExtensions.Geometry;

namespace Epic.Data.Battle
{
    public class BattleField
    {
        public SizeInt Size { get; }

        private readonly HexGrid<FieldCell> _cells;

        public BattleField(SizeInt size)
        {
            Size = size;
            _cells = new HexGrid<FieldCell>(Size.Width, Size.Height, HexGridType.HorizontalOdd, CreateDefaultCell);
        }

        private FieldCell CreateDefaultCell(int q, int r)
        {
            return new FieldCell(new HexPoint(q, r), FieldTile.Default);
        }
    }
}
