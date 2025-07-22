using System;

namespace Epic.Core
{
    public struct HexoPoint : IEquatable<HexoPoint>
    {
        public int C { get; set; }
        public int R { get; set; }

        public HexoPoint(int c, int r)
        {
            C = c;
            R = r;
        }
        
        public bool Equals(HexoPoint other)
        {
            return C == other.C && R == other.R;
        }

        public override bool Equals(object obj)
        {
            return obj is HexoPoint other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(C, R);
        }

        public static bool operator ==(HexoPoint left, HexoPoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HexoPoint left, HexoPoint right)
        {
            return !left.Equals(right);
        }
    }
}