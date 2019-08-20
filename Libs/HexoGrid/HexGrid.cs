namespace HexoGrid
{
    public class HexGrid
    {
        public int Width { get; }
        public int Height { get; }
        public HexGridType GridType { get; }

        public HexPoint this[int q, int r] => _points[q, r];

        private readonly HexPoint[,] _points;

        public HexGrid(int width, int height, HexGridType gridType)
        {
            Width = width;
            Height = height;
            GridType = gridType;

            _points = new HexPoint[Width, Height];
            for (int q = 0; q < Width; q++)
            {
                for (int r = 0; r < Height; r++)
                    _points[q,r] = new HexPoint(q, r);
            }
        }

        public override string ToString()
        {
            return $"{GridType}:{Width}x{Height}";
        }
    }
}
