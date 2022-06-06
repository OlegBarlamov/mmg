using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NetExtensions.Collections;

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

    public IEnumerable<Point3D> EnumeratePoints()
    {
      for (int x = Left; x <= Right; x++)
      {
        for (int y = Top; y <= Bottom; y++)
        {
          for (int z = Forward; z <= Backward; z++)
            yield return new Point3D(x, y, z);
        }
      }
    }
    
    public bool Intersects(RectangleBox value)
    {
      return value.Left < this.Right && this.Left < value.Right && value.Top < this.Bottom && this.Top < value.Bottom && value.Forward < this.Backward && this.Forward < value.Backward;
    }
    
    public static RectangleBox Intersect(RectangleBox value1, RectangleBox value2)
    {
      if (value1.Intersects(value2))
      {
        int num1 = Math.Min(value1.X + value1.Width, value2.X + value2.Width);
        int x = Math.Max(value1.X, value2.X);
        int y = Math.Max(value1.Y, value2.Y);
        int num2 = Math.Min(value1.Y + value1.Height, value2.Y + value2.Height);
        int num3 = Math.Min(value1.Z + value1.Depth, value2.Z + value2.Depth);
        int z = Math.Max(value1.Z, value2.Z);
        return new RectangleBox(x, y, z, num1 - x, num2 - y, num3 - z);
      }
      else
        return Empty;
    }

    public static RectangleBox FromStartEnd(Point3D start, Point3D end)
    {
      return new RectangleBox(start, end - start);
    }
    
    public static RectangleBox FromCenterAndRadius(Point3D center, Point3D radius)
    {
      return new RectangleBox(center.X - radius.X, center.Y - radius.Y, center.Z - radius.Z, center.X + radius.X, center.Y + radius.Y, center.Z + radius.Z);
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