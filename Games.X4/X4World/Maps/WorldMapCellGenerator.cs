using System;
using System.Collections.Generic;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using NetExtensions.Geometry;
using X4World.Generation;
using X4World.Objects;

namespace X4World.Maps
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
            var cfg = GalaxyConfig.Instance.MapCell;
            var map = GenerateData(data, cfg);
            var aggregatedData = new WorldMapCellAggregatedData(map);
            var position = GlobalWorldMap.WorldFromMapPoint(point);

            var content = new WorldMapCellContent(point, position, cfg.Node.CellSize, aggregatedData);
            return new GlobalWorldMapCell(point, content);
        }

        private IReadOnlyCollection<WorldMapCellAggregatedData.GalaxyPointData> GenerateData(byte data, MapCellConfig cfg)
        {
            var result = new List<WorldMapCellAggregatedData.GalaxyPointData>();

            if (cfg.Debug.SingleGalaxy)
            {
                var pos = cfg.Debug.Position;
                result.Add(new WorldMapCellAggregatedData.GalaxyPointData(
                    new Vector3(pos[0], pos[1], pos[2]), cfg.Debug.Temperature, cfg.Debug.Power));
                return result;
            }

            var gen = cfg.Generation;
            for (byte i = 0; i < data; i++)
            {
                var halfSize = cfg.Node.CellSize / 2;
                var position = RandomService.NextVector3(
                    new Vector3(-halfSize),
                    new Vector3(halfSize));
                var temperature = RandomService.NextFloat(gen.TemperatureMin, gen.TemperatureMax);
                var power = RandomService.NextFloat(gen.PowerMin, gen.PowerMax);

                result.Add(new WorldMapCellAggregatedData.GalaxyPointData(position, temperature, power));
            }

            return result;
        }
    }
}
