using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;
using MonoGameExtensions.Map;
using NetExtensions.Geometry;
using X4World.Objects;

namespace X4World.Maps
{
    public class GalaxiesMapCell : IMapCell<Point3D>
    {
        public Point3D MapPoint { get; }
        
        public Vector3 World { get; }
        
        public Vector3 Size { get; } = new Vector3(WorldConstants.GalaxiesMapCellSize);

        public IReadOnlyList<Galaxy> Galaxies {get; }

        private readonly List<Galaxy> _galaxies;

        public GalaxiesMapCell(Point3D mapPoint) : this(mapPoint, new Galaxy[0])
        {
        }
        
        public GalaxiesMapCell(Point3D mapPoint, IReadOnlyList<Galaxy> stars)
        {
            MapPoint = mapPoint;
            World = mapPoint.ToVector3() * Size;
            
            _galaxies = new List<Galaxy>(stars);
            Galaxies = _galaxies;
        }

        public void AddGalaxy([NotNull] Galaxy galaxy)
        {
            if (galaxy == null) throw new ArgumentNullException(nameof(galaxy));
            _galaxies.Add(galaxy);
        }
        
        public Point3D GetPointOnMap()
        {
            return MapPoint;
        }

        public bool ContainsPoint(Vector3 point)
        {
            return point.X > World.X && point.X < World.X + WorldConstants.GalaxiesMapCellSize * 2 &&
                   point.Y > World.Y && point.Y < World.Y + WorldConstants.GalaxiesMapCellSize * 2 &&
                   point.Z > World.Z && point.Z < World.Z + WorldConstants.GalaxiesMapCellSize * 2;
        }
    }
}