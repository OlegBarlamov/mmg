using FrameworkSDK.Common;
using Microsoft.Xna.Framework;
using NetExtensions.Geometry;
using X4World.Maps;
using X4World.Objects;

namespace X4World.Generation
{
    public interface IWorldMapCellGenerator
    {
        GlobalWorldMapCell Generate(Point3D point, byte data);
    }
    
    public class WorldMapCellGenerator : IWorldMapCellGenerator
    {
        public IRandomSeedProvider SeedProvider { get; }

        public WorldMapCellGenerator(IRandomSeedProvider seedProvider)
        {
            SeedProvider = seedProvider;
        }
        public GlobalWorldMapCell Generate(Point3D point, byte data)
        {
            var map = GenerateData(data);
            var aggregatedData = new WorldMapCellAggregatedData(map);
            var position = new Vector3(
                point.X * WorldConstants.WorldMapCellSize,
                point.Y * WorldConstants.WorldMapCellSize,
                point.Z * WorldConstants.WorldMapCellSize);
            
            var content = new WorldMapCellContent(point, position, WorldConstants.WorldMapCellSize, aggregatedData);
            return new GlobalWorldMapCell(point, content);
        }

        private byte[,,] GenerateData(byte data)
        {
            var map = new byte[25,25,25];
            for (int i = 0; i < data; i++)
            {
                var pos = new Point3D(SeedProvider.Seed.Next(0, 25),SeedProvider.Seed.Next(0, 25),SeedProvider.Seed.Next(0, 25));
                map[pos.X, pos.Y, pos.Z]++;
            }
            return map;
        }
    }
}