using System.Threading;
using System.Threading.Tasks;
using NetExtensions.Geometry;

namespace Atom.Client.MacOS.Services
{
    public interface IAstronomicMapGenerator
    {
        Task<AstronomicalMap> GenerateMapAsync(Point3D center, Point3D radius, CancellationToken cancellationToken);

        AstronomicalMapCell GenerateCell(Point3D point);
    }
}