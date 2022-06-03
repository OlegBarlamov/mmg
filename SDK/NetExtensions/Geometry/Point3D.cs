using System;
using System.Runtime.Serialization;

namespace NetExtensions.Geometry
{
  [DataContract]
  public struct Point3D : IEquatable<Point3D>
  {
    private static readonly Point3D zeroPoint;
    [DataMember] public int X;
    [DataMember] public int Y;
    [DataMember] public int Z;

    public static Point3D Zero
    {
      get { return Point3D.zeroPoint; }
    }

    internal string DebugDisplayString
    {
      get { return this.X.ToString() + "  " + this.Y.ToString() + "  " + this.Z.ToString(); }
    }

    public Point3D(int x, int y, int z)
    {
      this.X = x;
      this.Y = y;
      this.Z = z;
    }

    public Point3D(int value)
    {
      this.X = value;
      this.Y = value;
      this.Z = value;
    }

    public static Point3D operator +(Point3D value1, Point3D value2)
    {
      return new Point3D(value1.X + value2.X, value1.Y + value2.Y, value1.Z + value2.Z);
    }

    public static Point3D operator -(Point3D value1, Point3D value2)
    {
      return new Point3D(value1.X - value2.X, value1.Y - value2.Y, value1.Z - value2.Z);
    }

    public static Point3D operator *(Point3D value1, Point3D value2)
    {
      return new Point3D(value1.X * value2.X, value1.Y * value2.Y, value1.Z * value2.Z);
    }

    public static Point3D operator /(Point3D source, Point3D divisor)
    {
      return new Point3D(source.X / divisor.X, source.Y / divisor.Y, source.Z / divisor.Z);
    }

    public static bool operator ==(Point3D a, Point3D b)
    {
      return a.Equals(b);
    }

    public static bool operator !=(Point3D a, Point3D b)
    {
      return !a.Equals(b);
    }

    public override bool Equals(object obj)
    {
      return obj is Point3D other && this.Equals(other);
    }

    public bool Equals(Point3D other)
    {
      return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
    }

    public override string ToString()
    {
      return "{X:" + (object) this.X + " Y:" + (object) this.Y + " Z:" + (object) this.Z + "}";
    }

    public void Deconstruct(out int x, out int y, out int z)
    {
      x = this.X;
      y = this.Y;
      z = this.Z;
    }

    public override int GetHashCode()
    {
      int result = (int) (this.X ^ (this.X >> 32));
      result = 31 * result + (int) (this.Y ^ (this.Y >> 32));
      result = 31 * result + (int) (this.Z ^ (this.Z >> 32));
      return result;
    }
  }
}