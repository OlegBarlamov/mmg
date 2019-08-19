using System;

namespace HexoGrid
{
    public struct HexPoint : IEquatable<HexPoint>
    {
        private static readonly HexPoint ZeroPoint = new HexPoint();

        public readonly int X;
        public readonly int Y;

        public static HexPoint Zero => HexPoint.ZeroPoint;

        public override string ToString()
        {
            return $"{{X:{X} Y:{Y}}}";
        }

        public HexPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static HexPoint operator +(HexPoint value1, HexPoint value2)
        {
            return new HexPoint(value1.X + value2.X, value1.Y + value2.Y);
        }

        public static HexPoint operator -(HexPoint value1, HexPoint value2)
        {
            return new HexPoint(value1.X - value2.X, value1.Y - value2.Y);
        }

        public static HexPoint operator *(HexPoint value1, HexPoint value2)
        {
            return new HexPoint(value1.X * value2.X, value1.Y * value2.Y);
        }

        public static HexPoint operator /(HexPoint value1, HexPoint value2)
        {
            return new HexPoint(value1.X / value2.X, value1.Y / value2.Y);
        }

        public static bool operator ==(HexPoint a, HexPoint b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(HexPoint a, HexPoint b)
        {
            return !(a == b);
        }

        public bool Equals(HexPoint other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is HexPoint other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                return hashCode;
            }
        }
    }
}
