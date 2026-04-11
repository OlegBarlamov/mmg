using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxyTextureFarthestDetailsGenerator : IDetailsGenerator<GalaxyTextureFarthest>
    {
        private const int SubClustersPerPoint = 2;
        private const float SubClusterSpread = 0.08f;
        private const float SubClusterLuminosityScale = 0.4f;
        private const int SectorGridSize = 5;

        public void Generate(GalaxyTextureFarthest target)
        {
            var aggData = target.AggregatedData;
            var rng = new Random(aggData.Seed);

            var expandedPoints = ExpandClusterPoints(aggData.ClusterPoints, aggData.DiskRadius, rng);
            var sectors = PartitionIntoSectors(expandedPoints, aggData.DiskRadius, rng);

            var sectorSeed = rng.Next();
            var layerSeeds = new[] { rng.Next(), rng.Next(), rng.Next() };
            var layerStarCounts = new[] { 1500, 2000, 1500 };

            var layeredData = new GalaxyTextureLayeredAggregatedData(
                aggData.GalaxyColor,
                aggData.ArmCount,
                aggData.DiskRadius,
                aggData.Inclination,
                aggData.SpinAngle,
                sectorSeed,
                sectors,
                layerSeeds,
                layerStarCounts);

            var layered = new GalaxyTextureLayered(target, Vector3.Zero, layeredData);

            target.SetGeneratedData(new[] { layered });
        }

        private static GalaxySectorDefinition[] PartitionIntoSectors(
            GalaxyClusterPoint[] points, float diskRadius, Random rng)
        {
            var cellSize = diskRadius * 2f / SectorGridSize;
            var halfDisk = diskRadius;
            var buckets = new Dictionary<int, List<GalaxyClusterPoint>>();

            foreach (var p in points)
            {
                var col = (int)((p.X + halfDisk) / cellSize);
                var row = (int)((p.Z + halfDisk) / cellSize);
                col = Math.Max(0, Math.Min(col, SectorGridSize - 1));
                row = Math.Max(0, Math.Min(row, SectorGridSize - 1));

                var key = row * SectorGridSize + col;
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
                var row = kvp.Key / SectorGridSize;
                var col = kvp.Key % SectorGridSize;
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
            GalaxyClusterPoint[] originals, float diskRadius, Random rng)
        {
            var expanded = new List<GalaxyClusterPoint>(originals.Length * (1 + SubClustersPerPoint));

            foreach (var cluster in originals)
            {
                expanded.Add(cluster);

                var spread = diskRadius * SubClusterSpread;
                for (int s = 0; s < SubClustersPerPoint; s++)
                {
                    var ox = cluster.X + (float)(rng.NextDouble() - 0.5) * 2f * spread;
                    var oy = cluster.Y + (float)(rng.NextDouble() - 0.5) * 2f * spread * 0.2f;
                    var oz = cluster.Z + (float)(rng.NextDouble() - 0.5) * 2f * spread;

                    var tempShift = (float)(rng.NextDouble() - 0.5) * 2000f;
                    var subTemp = MathHelper.Clamp(cluster.Temperature + tempShift, 1000f, 10000f);
                    var subLum = cluster.Luminosity * (SubClusterLuminosityScale + (float)rng.NextDouble() * SubClusterLuminosityScale);

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
