using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
using MonoGameExtensions.Geometry;
using NetExtensions.Geometry;

namespace X4World.Maps
{
    public class GalaxiesMap : DictionaryBasedMap<Point3D, GalaxiesMapCell>
    {
        public GalaxiesMapCell FindPoint(Vector3 worldPoint)
        {
            var point = Point3DExtensions.Point3DFromVector3((worldPoint - new Vector3(WorldConstants.GalaxiesMapCellSize / 2)) / WorldConstants.GalaxiesMapCellSize);
            return GetCell(point);
        }

        public IEnumerable<KeyValuePair<Point3D, GalaxiesMapCell>> EnumerateCells(Point3D start, Point3D end)
        {
            for (int x = start.X; x <= end.X; x++)
            {
                for (int y = start.Y; y <= end.Y; y++)
                {
                    for (int z = start.Z; z <= end.Z; z++)
                    {
                        var point = new Point3D(x, y, z);
                        yield return new KeyValuePair<Point3D, GalaxiesMapCell>(point, GetCell(point));
                    }
                }
            }
        }

        public IEnumerable<KeyValuePair<Point3D, GalaxiesMapCell>> EnumerateCells()
        {
            return GetCells();
        }
    }
}