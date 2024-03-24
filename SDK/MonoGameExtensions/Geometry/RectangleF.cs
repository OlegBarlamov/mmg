using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using NetExtensions.Geometry;

namespace MonoGameExtensions.Geometry
{
  [DataContract]
  public struct RectangleF : IEquatable<RectangleF>
  {
    [DataMember] public float X;
    [DataMember] public float Y;

    [DataMember] public float Width;
    [DataMember] public float Height;

    public static RectangleF Empty
    {
      get { return new RectangleF(); }
    }

    public float Left
    {
      get { return X; }
    }

    public float Right
    {
      get { return X + Width; }
    }

    public float Top
    {
      get { return Y; }
    }

    public float Bottom
    {
      get { return Y + Height; }
    }

    public bool IsEmpty
    {
      get { return Width == 0 && Height == 0 && Location == Vector2.Zero; }
    }

    public Vector2 Location
    {
      get { return new Vector2(X, Y); }
      set
      {
        X = value.X;
        Y = value.Y;
      }
    }

    public Vector2 Size
    {
      get { return new Vector2(Width, Height); }
      set
      {
        Width = value.X;
        Height = value.Y;
      }
    }

    public Vector2 Start
    {
      get
      {
        return new Vector2(X, Y);
      }
    }

    public Vector2 End
    {
      get
      {
        return new Vector2(X + Width, Y + Height);
      }
    }

    public Vector2 Center
    {
      get { return new Vector2(X + Width / 2 + Y + Height / 2 ); }
    }

    public RectangleF(float x, float y, float width, float height)
    {
      X = x;
      Y = y;
      Width = width;
      Height = height;
    }

    public bool Intersects(RectangleF value)
    {
      return value.Left < this.Right && this.Left < value.Right && value.Top < this.Bottom && this.Top < value.Bottom;
    }

    public static RectangleF FromTopLeftAndSize(Vector2 topLeft, Vector2 size)
    {
      return new RectangleF(topLeft.X, topLeft.Y, size.X, size.Y);
    }

    public static RectangleF FromCenterAndSize(Vector2 center, Vector2 size)
    {
      return FromTopLeftBottomDown(center - size, center + size);
    }
    
    public static RectangleF FromCenterAndSize(Vector2 center, float size)
    {
      var vectorSize = new Vector2(size);
      return FromTopLeftBottomDown(center - vectorSize, center + vectorSize);
    }

    public static RectangleF FromTopLeftBottomDown(Vector2 topLeft, Vector2 bottomDown)
    {
      return new RectangleF(topLeft.X, topLeft.Y, bottomDown.X - topLeft.X, bottomDown.Y - topLeft.Y);
    }
    
    public static RectangleF Intersect(RectangleF value1, RectangleF value2)
    {
      if (value1.Intersects(value2))
      {
        float num1 = Math.Min(value1.X + value1.Width, value2.X + value2.Width);
        float x = Math.Max(value1.X, value2.X);
        float y = Math.Max(value1.Y, value2.Y);
        float num2 = Math.Min(value1.Y + value1.Height, value2.Y + value2.Height);
        return new RectangleF(x, y, num1 - x, num2 - y);
      }
      else
        return Empty;
    }
    

    public static bool operator ==(RectangleF a, RectangleF b)
    {
      return a.Equals(b);
    }

    public static bool operator !=(RectangleF a, RectangleF b)
    {
      return !a.Equals(b);
    }

    public bool Contains(float x, float y)
    {
      return x >= Left && x <= Right && y >= Top && y <= Bottom;
    }

    public bool Contains(Vector2 value)
    {
      return Contains(value.X, value.Y);
    }

    public void Contains(ref Vector2 value, out bool result)
    {
      result = Contains(value);
    }

    public bool Contains(RectangleF value)
    {
      return value.Left >= Left && value.Right <= Right && value.Top >= Top && value.Bottom <= Bottom;
    }

    public void Contains(ref RectangleF value, out bool result)
    {
      result = Contains(value);
    }

    public override bool Equals(object obj)
    {
      return obj is RectangleBox other && this.Equals(other);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = X.GetHashCode();
        hashCode = (hashCode * 397) ^ Y.GetHashCode();
        hashCode = (hashCode * 397) ^ Width.GetHashCode();
        hashCode = (hashCode * 397) ^ Height.GetHashCode();
        return hashCode;
      }
    }

    public bool Equals(RectangleF other)
    {
      return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height; 
    }

    public override string ToString()
    {
      return $"X: {X}, Y: {Y}, Width: {Width}, Height: {Height}";
    }

    public void Deconstruct(out float x, out float y, out float width, out float height)
    {
      x = X;
      y = Y;
      width = Width;
      height = Height;
    }
  }
}