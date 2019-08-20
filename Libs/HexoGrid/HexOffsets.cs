namespace HexoGrid
{
    public static class HexOffsets
    {
        public interface IHorizontalOffsets
        {
            HexPoint UpLeft { get; }
            HexPoint UpRight { get; }
            HexPoint Right { get; }
            HexPoint DownRight { get; }
            HexPoint DownLeft { get; }
            HexPoint Left { get; }
        }

        public static class HorizontalOdd
        {
            public static IHorizontalOffsets EvenRow { get; } = new EvenRowHorizontalOffsets();
            public static IHorizontalOffsets OddRow { get; } = new OddRowHorizontalOffsets();

            private class EvenRowHorizontalOffsets : IHorizontalOffsets
            {
                public HexPoint UpLeft { get; } = new HexPoint(-1, -1);
                public HexPoint UpRight { get; } = new HexPoint(0, -1);
                public HexPoint Right { get; } = new HexPoint(1, -1);
                public HexPoint DownRight { get; } = new HexPoint(0, 1);
                public HexPoint DownLeft { get; } = new HexPoint(-1, 1);
                public HexPoint Left { get; } = new HexPoint(-1, 0);
            }

            private class OddRowHorizontalOffsets : IHorizontalOffsets
            {
                public HexPoint UpLeft { get; } = new HexPoint(0, -1);
                public HexPoint UpRight { get; } = new HexPoint(1, -1);
                public HexPoint Right { get; } = new HexPoint(1, 0);
                public HexPoint DownRight { get; } = new HexPoint(1, 1);
                public HexPoint DownLeft { get; } = new HexPoint(0, 1);
                public HexPoint Left { get; } = new HexPoint(-1, 0);
            }
        }

        public static class HorizontalEven
        {
            public static IHorizontalOffsets EvenRow { get; } = new EvenRowHorizontalOffsets();
            public static IHorizontalOffsets OddRow { get; } = new OddRowHorizontalOffsets();

            private class EvenRowHorizontalOffsets : IHorizontalOffsets
            {
                public HexPoint UpLeft { get; } = new HexPoint(0, -1);
                public HexPoint UpRight { get; } = new HexPoint(1, -1);
                public HexPoint Right { get; } = new HexPoint(1, 0);
                public HexPoint DownRight { get; } = new HexPoint(1, 1);
                public HexPoint DownLeft { get; } = new HexPoint(0, 1);
                public HexPoint Left { get; } = new HexPoint(-1, 0);
            }

            private class OddRowHorizontalOffsets : IHorizontalOffsets
            {
                public HexPoint UpLeft { get; } = new HexPoint(-1, -1);
                public HexPoint UpRight { get; } = new HexPoint(0, -1);
                public HexPoint Right { get; } = new HexPoint(1, 0);
                public HexPoint DownRight { get; } = new HexPoint(0, 1);
                public HexPoint DownLeft { get; } = new HexPoint(-1, 1);
                public HexPoint Left { get; } = new HexPoint(-1, 0);
            }
        }

        public interface IVerticalOffsets
        {
            HexPoint Up { get; }
            HexPoint UpRight { get; }
            HexPoint DownRight { get; }
            HexPoint Down { get; }
            HexPoint DownLeft { get; } 
            HexPoint UpLeft { get; }
        }

        public static class VerticalOdd
        {
            public static IVerticalOffsets EvenCol { get; } = new EvenColVerticalOffsets();
            public static IVerticalOffsets OddCol { get; } = new OddColVerticalOffsets();

            private class EvenColVerticalOffsets : IVerticalOffsets
            {
                public HexPoint Up { get; } = new HexPoint(0, -1);
                public HexPoint UpRight { get; } = new HexPoint(1, -1);
                public HexPoint DownRight { get; } = new HexPoint(1, 0);
                public HexPoint Down { get; } = new HexPoint(0, 1);
                public HexPoint DownLeft { get; } = new HexPoint(-1, 0);
                public HexPoint UpLeft { get; } = new HexPoint(-1, -1);
            }

            private class OddColVerticalOffsets : IVerticalOffsets
            {
                public HexPoint Up { get; } = new HexPoint(0, -1);
                public HexPoint UpRight { get; } = new HexPoint(1, 0);
                public HexPoint DownRight { get; } = new HexPoint(1, 1);
                public HexPoint Down { get; } = new HexPoint(0, 1);
                public HexPoint DownLeft { get; } = new HexPoint(-1, 1);
                public HexPoint UpLeft { get; } = new HexPoint(-1, 0);
            }
        }

        public static class VerticalEven
        {
            public static IVerticalOffsets EvenCol { get; } = new EvenColVerticalOffsets();
            public static IVerticalOffsets OddCol { get; } = new OddColVerticalOffsets();

            private class EvenColVerticalOffsets : IVerticalOffsets
            {
                public HexPoint Up { get; } = new HexPoint(0, -1);
                public HexPoint UpRight { get; } = new HexPoint(1, 0);
                public HexPoint DownRight { get; } = new HexPoint(1, 1);
                public HexPoint Down { get; } = new HexPoint(0, 1);
                public HexPoint DownLeft { get; } = new HexPoint(-1, 1);
                public HexPoint UpLeft { get; } = new HexPoint(-1, 0);
            }

            private class OddColVerticalOffsets : IVerticalOffsets
            {
                public HexPoint Up { get; } = new HexPoint(0, -1);
                public HexPoint UpRight { get; } = new HexPoint(1, -1);
                public HexPoint DownRight { get; } = new HexPoint(1, 0);
                public HexPoint Down { get; } = new HexPoint(0, 1);
                public HexPoint DownLeft { get; } = new HexPoint(-1, 0);
                public HexPoint UpLeft { get; } = new HexPoint(-1, -1);
            }
        }
    }
}