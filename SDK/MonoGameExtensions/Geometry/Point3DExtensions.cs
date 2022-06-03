using Microsoft.Xna.Framework;
using NetExtensions.Geometry;

namespace MonoGameExtensions.Geometry
{
    public static class Point3DExtensions
    {
        public static Vector3 ToVector3(this Point3D point)
        {
            return new Vector3((float) point.X, (float) point.Y, (float) point.Z);
        }
    
        public static Point3D Point3DFromVector3(Vector3 vector)
        {
            return new Point3D((int)vector.X, (int)vector.Y, (int)vector.Z);
        }
    }

}