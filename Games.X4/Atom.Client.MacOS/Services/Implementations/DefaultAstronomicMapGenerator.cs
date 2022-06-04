using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using NetExtensions.Geometry;

namespace Atom.Client.MacOS.Services.Implementations
{
    [UsedImplicitly]
    internal class DefaultAstronomicMapGenerator : IAstronomicMapGenerator
    {
        private IRandomService RandomService { get; }

        public DefaultAstronomicMapGenerator([NotNull] IRandomService randomService)
        {
            RandomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
        }
        
        public Task<AstronomicalMap> GenerateMapAsync(Point3D center, Point3D radius, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    return GenerateMap(center, radius, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // ignored
                    return null;
                }
            }, TaskCreationOptions.LongRunning);
        }

        private AstronomicalMap GenerateMap(Point3D center, Point3D radius, CancellationToken cancellationToken)
        {
            var map = new AstronomicalMap();
            var initialMapBox = RectangleBox.FromCenterAndRadius(center, radius);
            foreach (var point in initialMapBox.EnumeratePoints())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var cell = GenerateCell(point);
                map.SetCell(point, cell);
            }

            return map;
        }

        public AstronomicalMapCell GenerateCell(Point3D point)
        {
            var cell = new AstronomicalMapCell(point);

            var maxPos = cell.World + cell.Size;
            var minPos = cell.World - cell.Size;

            var stars = Enumerable.Range(0, 10).Select(i => new StarModel(RandomService.NextVector3(minPos, maxPos), point)).ToArray();
            foreach (var starModel in stars)
            {
                cell.AddStar(starModel);
            }

            return cell;
        }
    }
}