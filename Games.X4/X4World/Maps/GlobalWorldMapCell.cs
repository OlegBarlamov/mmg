using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
using MonoGameExtensions.Geometry;
using NetExtensions.Geometry;
using X4World.Generation;
using X4World.Objects;

namespace X4World.Maps
{
    public class GlobalWorldMapCell : IGridCell<Point3D>
    {
        public Point3D MapPoint { get; }

        public Vector3 PositionCenter { get; }

        public float Side { get; }

        public WorldMapCellContent Content { get; }

        public GlobalWorldMapCell(Point3D mapPoint, [NotNull] WorldMapCellContent content)
        {
            MapPoint = mapPoint;
            Side = GalaxyConfig.Instance.MapCell.Node.CellSize;
            PositionCenter = mapPoint.ToVector3() * Side;
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public Point3D GetPointOnMap()
        {
            return MapPoint;
        }

        public bool ContainsPoint(Vector3 point)
        {
            var half = Side / 2;
            return point.X > PositionCenter.X - half && point.X < PositionCenter.X + half &&
                   point.Y > PositionCenter.Y - half && point.Y < PositionCenter.Y + half &&
                   point.Z > PositionCenter.Z - half && point.Z < PositionCenter.Z + half;
        }
    }
}