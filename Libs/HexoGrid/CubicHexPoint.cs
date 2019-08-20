using System;

namespace HexoGrid
{
    public struct CubicHexPoint : IEquatable<CubicHexPoint>
    {
        public static CubicHexPoint Zero => CubicHexPoint.ZeroPoint;

        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        private static readonly CubicHexPoint ZeroPoint = new CubicHexPoint();

        public override string ToString()
        {
            return $"{{X:{X} Y:{Y} Z:{Z}}}";
        }

        public CubicHexPoint(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static CubicHexPoint operator +(CubicHexPoint value1, CubicHexPoint value2)
        {
            return new CubicHexPoint(value1.X + value2.X, value1.Y + value2.Y, value1.Z + value2.Z);
        }

        public static CubicHexPoint operator -(CubicHexPoint value1, CubicHexPoint value2)
        {
            return new CubicHexPoint(value1.X - value2.X, value1.Y - value2.Y, value1.Z - value2.Z);
        }

        public static CubicHexPoint operator *(CubicHexPoint value1, CubicHexPoint value2)
        {
            return new CubicHexPoint(value1.X * value2.X, value1.Y * value2.Y, value1.Z * value2.Z);
        }

        public static CubicHexPoint operator /(CubicHexPoint value1, CubicHexPoint value2)
        {
            return new CubicHexPoint(value1.X / value2.X, value1.Y / value2.Y, value1.Z / value2.Z);
        }

        public static bool operator ==(CubicHexPoint a, CubicHexPoint b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(CubicHexPoint a, CubicHexPoint b)
        {
            return !(a == b);
        }

        public bool Equals(CubicHexPoint other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is CubicHexPoint other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ Z;
                return hashCode;
            }
        }
    }
}
