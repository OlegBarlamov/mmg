using System;
using System.Collections.Generic;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
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
        private IRandomService RandomService { get; }

        public WorldMapCellGenerator([NotNull] IRandomService randomService)
        {
            RandomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
        }
        public GlobalWorldMapCell Generate(Point3D point, byte data)
        {
            var map = GenerateData(data);
            var aggregatedData = new WorldMapCellAggregatedData(map);
            var position = GlobalWorldMap.WorldFromMapPoint(point);
            
            var content = new WorldMapCellContent(point, position, WorldConstants.WorldMapCellSize, aggregatedData);
            return new GlobalWorldMapCell(point, content);
        }

        private IReadOnlyCollection<WorldMapCellAggregatedData.GalaxyPointData> GenerateData(byte data)
        {
            var result = new List<WorldMapCellAggregatedData.GalaxyPointData>();

            for (byte i = 0; i < data; i++)
            {
                var position = RandomService.NextVector3(
                    new Vector3(-WorldConstants.WorldMapCellSize / 2),
                    new Vector3(WorldConstants.WorldMapCellSize / 2));
                var temperature = RandomService.NextFloat(1000, 10000);

                var pointData = new WorldMapCellAggregatedData.GalaxyPointData(position, temperature);
                result.Add(pointData);
            } 
            
            return result;
        }
    }
}