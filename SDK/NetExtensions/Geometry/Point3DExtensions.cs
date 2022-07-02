namespace NetExtensions.Geometry
{
    public static class Point3DExtensions
    {
        public static bool InRange(this Point3D point, Point3D min, Point3D max)
        {
            return point.X >= min.X && point.Y >= min.Y && point.Z >= min.Z &&
                   point.X <= max.X && point.Y <= max.Y && point.Z >= max.Z;
        }
    }
}