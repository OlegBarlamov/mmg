using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.Geometry;
using MonoGameExtensions.Map;
using NetExtensions.Geometry;

namespace Atom.Client.MacOS
{
    public class StarModel
    {
        public Vector3 World { get; }
        
        public Point3D MapCell { get; }

        public StarModel(Vector3 position, Point3D mapCell)
        {
            World = position;
            MapCell = mapCell;
        }
    }
    
    public class AstronomicalMapCell : IMapCell<Point3D>
    {
        public const float CellSize = 100f;
        
        public Point3D MapPoint { get; }
        
        public Vector3 World { get; }
        
        public Vector3 Size { get; } = new Vector3(CellSize, CellSize, CellSize);

        public IReadOnlyList<StarModel> Stars => _stars;

        private readonly List<StarModel> _stars;

        public AstronomicalMapCell(Point3D mapPoint) : this(mapPoint, new StarModel[0])
        {
        }
        
        public AstronomicalMapCell(Point3D mapPoint, IReadOnlyList<StarModel> stars)
        {
            MapPoint = mapPoint;
            World = mapPoint.ToVector3() * Size;
            
            _stars = new List<StarModel>(stars);
        }

        public void AddStar([NotNull] StarModel starModel)
        {
            if (starModel == null) throw new ArgumentNullException(nameof(starModel));
            _stars.Add(starModel);
        }
        
        public Point3D GetPointOnMap()
        {
            return MapPoint;
        }
    }
    
    public class AstronomicalMap : DictionaryBasedMap<Point3D, AstronomicalMapCell>
    {
        public AstronomicalMapCell FindPoint(Vector3 worldPoint)
        {
            var point = Point3DExtensions.Point3DFromVector3(worldPoint / AstronomicalMapCell.CellSize);
            return GetCell(point);
        }

        public IEnumerable<(Point3D, AstronomicalMapCell)> EnumerateCells(Point3D start, Point3D end)
        {
            for (int x = start.X; x <= end.X; x++)
            {
                for (int y = start.Y; y <= end.Y; y++)
                {
                    for (int z = start.Z; z <= end.Z; z++)
                    {
                        var point = new Point3D(x, y, z);
                        yield return (point, GetCell(point));
                    }
                }
            }
        }

        public IEnumerable<(Point3D, AstronomicalMapCell)> EnumerateCells()
        {
            return GetCells().Select((pair => (pair.Key, pair.Value)));
        }
    }
}