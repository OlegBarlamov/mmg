using System;
using System.Runtime.Serialization;

namespace NetExtensions.Geometry
{
  [DataContract]
  public struct RectangleBox : IEquatable<RectangleBox>
  {
    [DataMember] public int X;
    [DataMember] public int Y;
    [DataMember] public int Z;

    /// <summary>
    /// X Y surface length
    /// </summary>
    [DataMember] public int Width;

    /// <summary>
    /// X Y surface width
    /// </summary>
    [DataMember] public int Height;

    /// <summary>
    /// X Z surface depth
    /// </summary>
    [DataMember] public int Depth;

    public static RectangleBox Empty
    {
      get { return new RectangleBox(); }
    }

    public int Left
    {
      get { return X; }
    }

    public int Right
    {
      get { return X + Width - 1; }
    }

    public int Top
    {
      get { return Y; }
    }

    public int Bottom
    {
      get { return Y + Height - 1; }
    }

    public int Forward
    {
      get { return Z; }
    }

    public int Backward
    {
      get { return Z + Depth - 1; }
    }

    public bool IsEmpty
    {
      get { return Width == 0 && Height == 0 && Depth == 0 && Location == Point3D.Zero; }
    }

    public Point3D Location
    {
      get { return new Point3D(X, Y, Z); }
      set
      {
        X = value.X;
        Y = value.Y;
        Z = value.Z;
      }
    }

    public Point3D Size
    {
      get { return new Point3D(Width, Height, Depth); }
      set
      {
        Width = value.X;
        Height = value.Y;
        Depth = value.Z;
      }
    }

    public Point3D Start
    {
      get
      {
        return new Point3D(X, Y, Z);
      }
    }

    public Point3D End
    {
      get
      {
        return new Point3D(X + Width - 1, Y + Height - 1, Z + Depth - 1);
      }
    }

    public Point3D Center
    {
      get { return new Point3D(X + Width / 2 + Y + Height / 2 + Z + Depth / 2); }
    }

    public RectangleBox(int x, int y, int z, int width, int height, int depth)
    {
      X = x;
      Y = y;
      Z = z;
      Width = width;
      Height = height;
      Depth = depth;
    }

    public RectangleBox(Point3D location, Point3D size)
    {
      X = location.X;
      Y = location.Y;
      Z = location.Z;

      Width = size.X;
      Height = size.Y;
      Depth = size.Z;
    }

    public static RectangleBox FromStartEnd(Point3D start, Point3D end)
    {
      return new RectangleBox(start, end - start);
    }
    
    public static RectangleBox FromCenterAndRadius(Point3D center, Point3D radius)
    {
      return new RectangleBox(center.X - radius.X, center.Y - radius.Y, center.Z - radius.Z, radius.X * 2 + 1, radius.Y * 2 + 1, radius.Z * 2 + 1);
    }

    public static bool operator ==(RectangleBox a, RectangleBox b)
    {
      return a.Equals(b);
    }

    public static bool operator !=(RectangleBox a, RectangleBox b)
    {
      return !a.Equals(b);
    }

    public bool Contains(int x, int y, int z)
    {
      return x >= Left && x <= Right && y >= Top && y <= Bottom && z >= Forward && z <= Backward;
    }

    public bool Contains(float x, float y, float z)
    {
      return x >= Left && x <= Right && y >= Top && y <= Bottom && z >= Forward && z <= Backward;
    }

    public bool Contains(Point3D value)
    {
      return Contains(value.X, value.Y, value.Z);
    }

    public void Contains(ref Point3D value, out bool result)
    {
      result = Contains(value);
    }

    public bool Contains(RectangleBox value)
    {
      return value.Left >= Left && value.Right <= Right && value.Top >= Top && value.Bottom <= Bottom &&
             value.Forward >= Forward && value.Backward <= Backward;
    }

    public void Contains(ref RectangleBox value, out bool result)
    {
      result = Contains(value);
    }

    public override bool Equals(object obj)
    {
      return obj is RectangleBox other && this.Equals(other);
    }

    public bool Equals(RectangleBox other)
    {
      return X == other.X && Y == other.Y && Z == other.Z && Width == other.Width && Height == other.Height &&
             Depth == other.Depth;
    }

    public override string ToString()
    {
      return $"X: {X}, Y: {Y}, Z: {Z}, Width: {Width}, Height: {Height}, Depth: {Depth}";
    }

    public void Deconstruct(out int x, out int y, out int z, out int width, out int height, out int depth)
    {
      x = X;
      y = Y;
      z = Z;
      width = Width;
      height = Height;
      depth = Depth;
    }
    
    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = X;
        hashCode = (hashCode * 397) ^ Y;
        hashCode = (hashCode * 397) ^ Z;
        hashCode = (hashCode * 397) ^ Width;
        hashCode = (hashCode * 397) ^ Height;
        hashCode = (hashCode * 397) ^ Depth;
        return hashCode;
      }
    }
  }
}