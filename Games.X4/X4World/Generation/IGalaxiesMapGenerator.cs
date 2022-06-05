using System.Threading;
using System.Threading.Tasks;
using NetExtensions.Geometry;
using X4World.Maps;

namespace X4World.Generation
{
    public interface IGalaxiesMapGenerator
    {
        Task<GalaxiesMap> GenerateMapAsync(Point3D center, Point3D radius, CancellationToken cancellationToken);

        GalaxiesMapCell GenerateCell(Point3D point);
    }
}