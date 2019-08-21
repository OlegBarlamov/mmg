using System;

namespace NetExtensions
{
    public struct Int32Size : IEquatable<Int32Size>
    {
        public int Width;
        public int Height;

        public Int32Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return $"{Width}x{Height}";
        }

        public bool Equals(Int32Size other)
        {
            return Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Int32Size other && Equals(other);
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