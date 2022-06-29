using System.Drawing;
using Microsoft.Xna.Framework;

namespace MonoGameExtensions.Geometry
{
    public static class PointFloatExtensions
    {
        public static Vector2 ToVector2(this PointF pointF)
        {
            return new Vector2(pointF.X, pointF.Y);
        }

        public static PointF ToPointF(this Vector2 vector)
        {
            return new PointF(vector.X, vector.Y);
        }
    }
}