using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
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

        public override GlobalWorldMapCell GetCell(Point3D point)
        {
            if (!Cells.ContainsKey(point))
            {
                // TODO multithreading??
                
                var cellData = MapData[point.X + MapData.GetLength(0) / 2, point.Y + MapData.GetLength(1) / 2, point.Z + + MapData.GetLength(2) / 2];
                var cell = CellGenerator.Generate(point, cellData);
                base.SetCell(point, cell);
                return cell;
            }
            
            return base.GetCell(point);
        }

        public GlobalWorldMapCell FindPoint(Vector3 worldPoint)
        {
            var point = Point3DExtensions.Point3DFromVector3((worldPoint - new Vector3(WorldConstants.WorldMapCellSize / 2)) / WorldConstants.WorldMapCellSize);
            return GetCell(point);
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

        public IEnumerable<KeyValuePair<Point3D, GlobalWorldMapCell>> EnumerateCells()
        {
            return GetCells();
        }
    }
}