using System;
using FrameworkSDK.Common;
using JetBrains.Annotations;
using MonoGameExtensions.DataStructures;
using NetExtensions.Geometry;
using NetExtensions.Helpers;
using X4World.Generation;

namespace X4World.Maps
{
    public interface IWorldMapGenerator
    {
        IGrid<Point3D, GlobalWorldMapCell> Generate();
    }
    
    public class WorldMapGenerator : IWorldMapGenerator
    {
        public IWorldMapCellGenerator CellGenerator { get; }
        public IRandomSeedProvider RandomSeedProvider { get; }

        public WorldMapGenerator([NotNull] IWorldMapCellGenerator cellGenerator, [NotNull] IRandomSeedProvider randomSeedProvider)

        {
            CellGenerator = cellGenerator ?? throw new ArgumentNullException(nameof(cellGenerator));
            RandomSeedProvider = randomSeedProvider ?? throw new ArgumentNullException(nameof(randomSeedProvider));
        }
        
        public IGrid<Point3D, GlobalWorldMapCell> Generate()
        {
            return new GlobalWorldMap(GenerateData(), CellGenerator);
        }

        private byte[,,] GenerateData()
        {
            if (GalaxyConfig.Instance.MapCell.Debug.SingleGalaxy)
            {
                var data = new byte[1, 1, 1];
                data[0, 0, 0] = 1;
                return data;
            }
            return ArrayGenerator.GetRandomArray(10, 50, 100, 100, 100, RandomSeedProvider.Seed, i => (byte)i);
        }
    }
}
