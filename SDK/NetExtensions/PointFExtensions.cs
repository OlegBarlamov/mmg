using System;
using System.Drawing;

namespace NetExtensions
{
    public static class PointFExtensions
    {
        public static PointF Sub(this PointF point1, PointF point2)
        {
            return new PointF(point1.X - point2.X, point1.Y - point2.Y);
        }
        
        public static PointF Sum(this PointF point1, PointF point2)
        {
            return new PointF(point1.X + point2.X, point1.Y + point2.Y);
        }

        public static float AsVectorLength(this PointF point)
        {
            return (float)Math.Sqrt(point.X * point.X + point.Y * point.Y);
        }
    }
}