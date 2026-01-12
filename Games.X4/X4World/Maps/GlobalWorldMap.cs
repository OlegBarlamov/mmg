using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
using MonoGameExtensions.Geometry;
using NetExtensions.Geometry;
using X4World.Generation;
using Point3DExtensions = MonoGameExtensions.Geometry.Point3DExtensions;

namespace X4World.Maps
{
    public class GlobalWorldMap : DictionaryBasedGrid<Point3D, GlobalWorldMapCell>
    {
        public byte[,,] MapData { get; }
        public IWorldMapCellGenerator CellGenerator { get; }

        public GlobalWorldMap([NotNull] byte[,,] mapData, [NotNull] IWorldMapCellGenerator cellGenerator)
        {
            MapData = mapData ?? throw new ArgumentNullException(nameof(mapData));
            CellGenerator = cellGenerator ?? throw new ArgumentNullException(nameof(cellGenerator));
        }

        public static Point3D MapPointFromWorld(Vector3 worldPoint)
        {
            return Point3DExtensions.Point3DFromVector3(new Vector3(
                (worldPoint.X + Math.Sign(worldPoint.X) * WorldConstants.WorldMapCellSize / 2) / WorldConstants.WorldMapCellSize,
                (worldPoint.Y + Math.Sign(worldPoint.Y) * WorldConstants.WorldMapCellSize / 2) / WorldConstants.WorldMapCellSize,
                (worldPoint.Z + Math.Sign(worldPoint.Z) * WorldConstants.WorldMapCellSize / 2) / WorldConstants.WorldMapCellSize));
        }
        
        public static Vector3 WorldFromMapPoint(Point3D mapPoint)
        {
            return mapPoint.ToVector3() * WorldConstants.WorldMapCellSize;
        }

        public override GlobalWorldMapCell GetCell(Point3D point)
        {
            if (!Cells.ContainsKey(point))
            {
                // TODO multithreading??
                var mapDataPoint = new Point3D(point.X + MapData.GetLength(0) / 2, point.Y + MapData.GetLength(1) / 2, point.Z + MapData.GetLength(2) / 2);

                byte cellData = 0;
                if (mapDataPoint.X >= 0 && mapDataPoint.X < MapData.GetLength(0) &&
                    mapDataPoint.Y >= 0 && mapDataPoint.Y < MapData.GetLength(1) &&
                    mapDataPoint.Z >= 0 && mapDataPoint.Z < MapData.GetLength(2))
                {
                    cellData = MapData[mapDataPoint.X, mapDataPoint.Y, mapDataPoint.Z];
                }
                var cell = CellGenerator.Generate(point, cellData);
                base.SetCell(point, cell);
                return cell;
            }
            
            return base.GetCell(point);
        }

        public GlobalWorldMapCell FindPoint(Vector3 worldPoint)
        {
            return GetCell(MapPointFromWorld(worldPoint));
        }

        public IEnumerable<KeyValuePair<Point3D, GlobalWorldMapCell>> EnumerateCells(Point3D min, Point3D max)
        {
            for (int x = min.X; x <= max.X; x++)
            {
                for (int y = min.Y; y <= max.Y; y++)
                {
                    for (int z = min.Z; z <= max.Z; z++)
                    {
                        var point = new Point3D(x, y, z);
                        yield return new KeyValuePair<Point3D, GlobalWorldMapCell>(point, GetCell(point));
                    }
                }
            }
        }

        /// <summary>
        /// For radius 2 - picture like:
        /// 0 0 1 0 0
        /// 0 1 1 1 0
        /// 1 1 1 1 1
        /// 0 1 1 1 0
        /// 0 0 1 0 0 
        /// </summary>
        public IEnumerable<KeyValuePair<Point3D, GlobalWorldMapCell>> EnumerateCells(Point3D center, int radius)
        {
            var smallRectangleSize = new Point3D(radius - 1); 
            var min = center - smallRectangleSize;
            var max = center + smallRectangleSize;
            foreach (var cell in EnumerateCells(min, max))
                yield return cell;

            var point = new Point3D(min.X - 1, center.Y, center.Z);
            yield return new KeyValuePair<Point3D, GlobalWorldMapCell>(point, GetCell(point));
            point = new Point3D(center.X, min.Y - 1, center.Z);
            yield return new KeyValuePair<Point3D, GlobalWorldMapCell>(point, GetCell(point));
            point = new Point3D(center.X, center.Y, min.Z - 1);
            yield return new KeyValuePair<Point3D, GlobalWorldMapCell>(point, GetCell(point));
            point = new Point3D(max.X + 1, center.Y, center.Z);
            yield return new KeyValuePair<Point3D, GlobalWorldMapCell>(point, GetCell(point));
            point = new Point3D(center.X, max.Y + 1, center.Z);
            yield return new KeyValuePair<Point3D, GlobalWorldMapCell>(point, GetCell(point));
            point = new Point3D(center.X, center.Y, max.Z + 1);
            yield return new KeyValuePair<Point3D, GlobalWorldMapCell>(point, GetCell(point));
        }

        public IEnumerable<KeyValuePair<Point3D, GlobalWorldMapCell>> EnumerateCells()
        {
            return GetCells();
        }
    }
}