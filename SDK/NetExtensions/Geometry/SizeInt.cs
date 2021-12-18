using System;

namespace NetExtensions.Geometry
{
    public struct SizeInt : IEquatable<SizeInt>
    {
        public int Width;
        public int Height;

        public SizeInt(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return $"{Width}x{Height}";
        }

        public bool Equals(SizeInt other)
        {
            return Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SizeInt other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Width * 397) ^ Height;
            }
        }
    }
}