using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using X4World.Generation;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxyTextureLayeredDetailsGenerator : IDetailsGenerator<GalaxyTextureLayered>
    {
        public void Generate(GalaxyTextureLayered target)
        {
            var cfg = GalaxyConfig.Instance.GalaxyTextureLayered.Generation;
            var aggData = target.AggregatedData;
            var sectors = aggData.Sectors;
            var results = new List<IWrappedDetails>(sectors.Length);

            var galaxyRotation = Matrix.CreateRotationX(aggData.Inclination)
                               * Matrix.CreateRotationY(aggData.SpinAngle);

            var rng = new Random(aggData.Seed);

            for (int i = 0; i < sectors.Length; i++)
            {
                var sector = sectors[i];
                var localPos = Vector3.Transform(new Vector3(sector.CenterX, 0f, sector.CenterZ), galaxyRotation);

                var expandedPoints = ExpandClusterPoints(sector.ClusterPoints, sector.Radius, rng, cfg);

                var sectorTextureData = new GalaxySectorTextureAggregatedData(
                    aggData.GalaxyColor, sector.Radius, aggData.DiskRadius,
                    sector.CenterX, sector.CenterZ,
                    aggData.Inclination, aggData.SpinAngle,
                    sector.Seed, expandedPoints);
                var sectorTexture = new GalaxySectorTexture(target, localPos, sectorTextureData);
                results.Add(sectorTexture);
            }

            target.SetGeneratedData(results);
        }

        private static GalaxyClusterPoint[] ExpandClusterPoints(
            GalaxyClusterPoint[] originals, float sectorRadius, Random rng,
            GalaxyTextureLayeredGenerationConfig cfg)
        {
            if (cfg.SubClustersPerPoint <= 0)
                return originals;

            var expanded = new List<GalaxyClusterPoint>(originals.Length * (1 + cfg.SubClustersPerPoint));

            foreach (var cluster in originals)
            {
                expanded.Add(cluster);

                var spread = sectorRadius * cfg.SubClusterSpread;
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
            Generate((GalaxyTextureLayered)target);
        }
    }
}
