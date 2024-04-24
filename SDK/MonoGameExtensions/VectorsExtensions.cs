using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;
using NetExtensions.Geometry;
using NetExtensions.Helpers;

namespace MonoGameExtensions
{
    public static class VectorsExtensions
    {
        public static Point ToPoint(this Vector2 vector)
        {
            return new Point((int)vector.X, (int)vector.Y);
        }
        
        public static Point3D ToPoint(this Vector3 vector)
        {
            return new Point3D((int)vector.X, (int)vector.Y, (int)vector.Z);
        }

        public static Vector2 EmitZ(this Vector3 vector)
        {
            return new Vector2(vector.X, vector.Y);
        }

        public static Vector2 DecreaseMagnitude(this Vector2 vector2, float value)
        {
            var length = vector2.Length();
            if (length > value)
            {
                var reductionFactor = 1 - value / length;
                return vector2 * reductionFactor;
            }
            
            return Vector2.Zero;
        }
    }
}