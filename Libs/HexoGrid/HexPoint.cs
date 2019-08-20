using System;

namespace HexoGrid
{
    public struct HexPoint : IEquatable<HexPoint>
    {
        public static HexPoint Zero => HexPoint.ZeroPoint;

        /// <summary>
        /// Column number from 0
        /// </summary>
        public readonly int Q;
		/// <summary>
		/// Row number from 0
		/// </summary>
        public readonly int R;

        private static readonly HexPoint ZeroPoint = new HexPoint();

        public override string ToString()
        {
            return $"{{Q:{Q} R:{R}}}";
        }

        public HexPoint(int q, int r)
        {
	        Q = q;
            R = r;
        }

        public static HexPoint operator +(HexPoint value1, HexPoint value2)
        {
            return new HexPoint(value1.Q + value2.Q, value1.R + value2.R);
        }

        public static HexPoint operator -(HexPoint value1, HexPoint value2)
        {
            return new HexPoint(value1.Q - value2.Q, value1.R - value2.R);
        }

        public static HexPoint operator *(HexPoint value1, HexPoint value2)
        {
            return new HexPoint(value1.Q * value2.Q, value1.R * value2.R);
        }

        public static HexPoint operator /(HexPoint value1, HexPoint value2)
        {
            return new HexPoint(value1.Q / value2.Q, value1.R / value2.R);
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
            return Q == other.Q && R == other.R;
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
                var hashCode = Q;
                hashCode = (hashCode * 397) ^ R;
                return hashCode;
            }
        }
    }
}
