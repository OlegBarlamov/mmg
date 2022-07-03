using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
using MonoGameExtensions.Geometry;
using NetExtensions.Geometry;
using X4World.Objects;

namespace X4World.Maps
{
    public class GlobalWorldMapCell : IGridCell<Point3D>
    {
        public Point3D MapPoint { get; }

        public Vector3 PositionCenter { get; }
        
        /// <summary>
        /// Width/Height/Depth of the cell
        /// </summary>
        public float Side { get; } = WorldConstants.WorldMapCellSize;
        
        public WorldMapCellContent Content { get; }

        public GlobalWorldMapCell(Point3D mapPoint, [NotNull] WorldMapCellContent content)
        {
            MapPoint = mapPoint;
            PositionCenter = mapPoint.ToVector3() * Side;
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public Point3D GetPointOnMap()
        {
            return MapPoint;
        }

        public bool ContainsPoint(Vector3 point)
        {
            return point.X > PositionCenter.X - WorldConstants.WorldMapCellSize / 2 && point.X < PositionCenter.X + WorldConstants.WorldMapCellSize / 2 &&
                   point.Y > PositionCenter.Y - WorldConstants.WorldMapCellSize / 2 && point.Y < PositionCenter.Y + WorldConstants.WorldMapCellSize / 2 &&
                   point.Z > PositionCenter.Z - WorldConstants.WorldMapCellSize / 2 && point.Z < PositionCenter.Z + WorldConstants.WorldMapCellSize / 2;
        }
    }
}