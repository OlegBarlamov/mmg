using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
using MonoGameExtensions.Geometry;
using NetExtensions.Geometry;

namespace X4World.Maps
{
    public class GlobalWorldMap : DictionaryBasedMap<Point3D, GlobalWorldMapCell>
    {
        public GlobalWorldMapCell FindPoint(Vector3 worldPoint)
        {
            var point = Point3DExtensions.Point3DFromVector3((worldPoint - new Vector3(WorldConstants.GalaxiesMapCellSize / 2)) / WorldConstants.GalaxiesMapCellSize);
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