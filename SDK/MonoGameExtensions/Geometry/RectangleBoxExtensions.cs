using Microsoft.Xna.Framework;
using NetExtensions.Geometry;

namespace MonoGameExtensions.Geometry
{
    public static class RectangleBoxExtensions
    {
        public static bool Contains(this RectangleBox rectangle, Vector3 value)
        {
            return rectangle.Contains(value.X, value.Y, value.Z);
        }

        public static void Contains(this RectangleBox rectangle, ref Vector3 value, out bool result)
        {
            result = rectangle.Contains(value);
        }
    }
}