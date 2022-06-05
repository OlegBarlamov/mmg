using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using NetExtensions.Geometry;
using X4World.Maps;
using X4World.Objects;

namespace X4World.Generation
{
    [UsedImplicitly]
    public class DefaultGalaxiesMapGenerator : IGalaxiesMapGenerator
    {
        private IRandomService RandomService { get; }
        public IStarsMapGenerator StarsMapGenerator { get; }

        public DefaultGalaxiesMapGenerator([NotNull] IRandomService randomService, [NotNull] IStarsMapGenerator starsMapGenerator)
        {
            RandomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
            StarsMapGenerator = starsMapGenerator ?? throw new ArgumentNullException(nameof(starsMapGenerator));
        }
        
        public Task<GalaxiesMap> GenerateMapAsync(Point3D center, Point3D radius, CancellationToken cancellationToken)
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

        private GalaxiesMap GenerateMap(Point3D center, Point3D radius, CancellationToken cancellationToken)
        {
            var map = new GalaxiesMap();
            var initialMapBox = RectangleBox.FromCenterAndRadius(center, radius);
            foreach (var point in initialMapBox.EnumeratePoints())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var cell = GenerateCell(point, point == center);
                map.SetCell(point, cell);
            }

            return map;
        }

        public GalaxiesMapCell GenerateCell(Point3D point)
        {
            return GenerateCell(point, false);
        }
        
        public GalaxiesMapCell GenerateCell(Point3D point, bool withStars)
        {
            var cell = new GalaxiesMapCell(point);

            var maxPos = cell.World + cell.Size;
            var minPos = cell.World - cell.Size;

            var galaxies = Enumerable.Range(0, 10).Select(i => new Galaxy(cell, RandomService.NextVector3(minPos, maxPos))).ToArray();
            foreach (var galaxy in galaxies)
            {
                cell.AddGalaxy(galaxy);
                if (withStars)
                {
                    StarsMapGenerator.GenerateStarsForGalaxy(galaxy);
                }
            }

            return cell;
        }
    }
}