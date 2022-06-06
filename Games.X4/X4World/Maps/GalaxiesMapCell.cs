using Microsoft.Xna.Framework;
using MonoGameExtensions;
using MonoGameExtensions.DataStructures;
using MonoGameExtensions.Geometry;
using NetExtensions.Geometry;
using X4World.Objects;

namespace X4World.Maps
{
    public class GalaxiesMapCell : IMapCell<Point3D>
    {
        public Point3D MapPoint { get; }
        
        /// <summary>
        /// Center of the cell
        /// </summary>
        public Vector3 World { get; }
        
        /// <summary>
        /// Width/Height/Depth of the cell
        /// </summary>
        public float Size { get; } = WorldConstants.GalaxiesMapCellSize;

        public AutoSplitOctreeNode<Galaxy> GalaxiesTree { get; }

        public GalaxiesMapCell(Point3D mapPoint)
        {
            MapPoint = mapPoint;
            World = mapPoint.ToVector3() * Size;
            GalaxiesTree = new AutoSplitOctreeNode<Galaxy>(World, Size, 10);
        }
        
        public Point3D GetPointOnMap()
        {
            return MapPoint;
        }

        public bool ContainsPoint(Vector3 point)
        {
            return point.X > World.X - WorldConstants.GalaxiesMapCellSize / 2 && point.X < World.X + WorldConstants.GalaxiesMapCellSize / 2 &&
                   point.Y > World.Y - WorldConstants.GalaxiesMapCellSize / 2 && point.Y < World.Y + WorldConstants.GalaxiesMapCellSize / 2 &&
                   point.Z > World.Z - WorldConstants.GalaxiesMapCellSize / 2 && point.Z < World.Z + WorldConstants.GalaxiesMapCellSize / 2;
        }
    }
}