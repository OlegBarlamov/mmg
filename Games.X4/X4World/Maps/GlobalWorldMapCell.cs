using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
using MonoGameExtensions.Geometry;
using NetExtensions.Geometry;

namespace X4World.Maps
{
    public class GlobalWorldMapCell : IMapCell<Point3D>
    {
        public Point3D MapPoint { get; }

        public Vector3 PositionCenter { get; }
        
        /// <summary>
        /// Width/Height/Depth of the cell
        /// </summary>
        public float Side { get; } = WorldConstants.GalaxiesMapCellSize;
        
        public GlobalWorldMapCellContent Content { get; }

        public GlobalWorldMapCell(Point3D mapPoint)
        {
            MapPoint = mapPoint;
            PositionCenter = mapPoint.ToVector3() * Side;
            Content = new GlobalWorldMapCellContent(PositionCenter, Side);
        }

        public Point3D GetPointOnMap()
        {
            return MapPoint;
        }

        public bool ContainsPoint(Vector3 point)
        {
            return point.X > PositionCenter.X - WorldConstants.GalaxiesMapCellSize / 2 && point.X < PositionCenter.X + WorldConstants.GalaxiesMapCellSize / 2 &&
                   point.Y > PositionCenter.Y - WorldConstants.GalaxiesMapCellSize / 2 && point.Y < PositionCenter.Y + WorldConstants.GalaxiesMapCellSize / 2 &&
                   point.Z > PositionCenter.Z - WorldConstants.GalaxiesMapCellSize / 2 && point.Z < PositionCenter.Z + WorldConstants.GalaxiesMapCellSize / 2;
        }
    }
}