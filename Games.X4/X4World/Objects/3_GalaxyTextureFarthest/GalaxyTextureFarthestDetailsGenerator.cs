using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using X4World.Generation;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxyTextureFarthestDetailsGenerator : IDetailsGenerator<GalaxyTextureFarthest>
    {
        public void Generate(GalaxyTextureFarthest target)
        {
            var cfg = GalaxyConfig.Instance.GalaxyTextureFarthest;
            var aggData = target.AggregatedData;
            var rng = new Random(aggData.Seed);

            var expandedPoints = ExpandClusterPoints(aggData.ClusterPoints, aggData.DiskRadius, rng, cfg);
            var sectors = PartitionIntoSectors(expandedPoints, aggData.DiskRadius, rng, cfg.SectorGridSize);

            var sectorSeed = rng.Next();

            var layeredData = new GalaxyTextureLayeredAggregatedData(
                aggData.GalaxyColor,
                aggData.ArmCount,
                aggData.DiskRadius,
                aggData.Inclination,
                aggData.SpinAngle,
                sectorSeed,
                sectors);

            var layered = new GalaxyTextureLayered(target, Vector3.Zero, layeredData);

            target.SetGeneratedData(new[] { layered });
        }

        private static GalaxySectorDefinition[] PartitionIntoSectors(
            GalaxyClusterPoint[] points, float diskRadius, Random rng, int sectorGridSize)
        {
            var cellSize = diskRadius * 2f / sectorGridSize;
            var halfDisk = diskRadius;
            var buckets = new Dictionary<int, List<GalaxyClusterPoint>>();

            foreach (var p in points)
            {
                var col = (int)((p.X + halfDisk) / cellSize);
                var row = (int)((p.Z + halfDisk) / cellSize);
                col = Math.Max(0, Math.Min(col, sectorGridSize - 1));
                row = Math.Max(0, Math.Min(row, sectorGridSize - 1));

                var key = row * sectorGridSize + col;
                if (!buckets.TryGetValue(key, out var list))
                {
                    list = new List<GalaxyClusterPoint>();
                    buckets[key] = list;
                }
                list.Add(p);
            }

            var sectors = new List<GalaxySectorDefinition>(buckets.Count);
            var sectorRadius = cellSize * 0.5f;

            foreach (var kvp in buckets)
            {
                var row = kvp.Key / sectorGridSize;
                var col = kvp.Key % sectorGridSize;
                var cx = -halfDisk + (col + 0.5f) * cellSize;
                var cz = -halfDisk + (row + 0.5f) * cellSize;
                var seed = rng.Next();

                var rawPoints = kvp.Value;
                var localPoints = new GalaxyClusterPoint[rawPoints.Count];
                for (int i = 0; i < rawPoints.Count; i++)
                {
                    var p = rawPoints[i];
                    localPoints[i] = new GalaxyClusterPoint(p.X - cx, p.Y, p.Z - cz, p.Temperature, p.Luminosity);
                }

                sectors.Add(new GalaxySectorDefinition(cx, cz, sectorRadius, seed, localPoints));
            }

            return sectors.ToArray();
        }

        private static GalaxyClusterPoint[] ExpandClusterPoints(
            GalaxyClusterPoint[] originals, float diskRadius, Random rng, GalaxyTextureFarthestConfig cfg)
        {
            var expanded = new List<GalaxyClusterPoint>(originals.Length * (1 + cfg.SubClustersPerPoint));

            foreach (var cluster in originals)
            {
                expanded.Add(cluster);

                var spread = diskRadius * cfg.SubClusterSpread;
                for (int s = 0; s < cfg.SubClustersPerPoint; s++)
                {
                    var ox = cluster.X + (float)(rng.NextDouble() - 0.5) * 2f * spread;
                    var oy = cluster.Y + (float)(rng.NextDouble() - 0.5) * 2f * spread * 0.2f;
                    var oz = cluster.Z + (float)(rng.NextDouble() - 0.5) * 2f * spread;

                    var tempShift = (float)(rng.NextDouble() - 0.5) * 2000f;
                    var subTemp = MathHelper.Clamp(cluster.Temperature + tempShift, 1000f, 10000f);
                    var subLum = cluster.Luminosity * (cfg.SubClusterLuminosityScale + (float)rng.NextDouble() * cfg.SubClusterLuminosityScale);

                    expanded.Add(new GalaxyClusterPoint(ox, oy, oz, subTemp, subLum));
                }
            }

            return expanded.ToArray();
        }

        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((GalaxyTextureFarthest)target);
        }
    }
}
