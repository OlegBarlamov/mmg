using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxySectorTextureDetailsGenerator : IDetailsGenerator<GalaxySectorTexture>
    {
        private const int FillStarsPerPoint = 3;
        private const float FillSpreadFraction = 0.15f;
        private const float FillLuminosityScale = 0.3f;
        private const float DiskThicknessScale = 2.5f;

        public void Generate(GalaxySectorTexture target)
        {
            var aggData = target.AggregatedData;
            var rng = new Random(aggData.Seed);

            var expanded = ExpandClusterPoints(aggData.ClusterPoints, aggData.SectorRadius, rng);

            var sectorData = new GalaxySectorAggregatedData(
                aggData.GalaxyColor, aggData.SectorRadius,
                aggData.Inclination, aggData.SpinAngle,
                expanded);

            var sector = new GalaxySector(target, Vector3.Zero, aggData.SectorRadius * 3f, sectorData);
            target.SetGeneratedData(new List<IWrappedDetails> { sector });
        }

        private static GalaxyClusterPoint[] ExpandClusterPoints(
            GalaxyClusterPoint[] originals, float sectorRadius, Random rng)
        {
            var expanded = new List<GalaxyClusterPoint>(originals.Length * (1 + FillStarsPerPoint));
            var spread = sectorRadius * FillSpreadFraction;

            foreach (var p in originals)
            {
                expanded.Add(new GalaxyClusterPoint(
                    p.X, p.Y * DiskThicknessScale, p.Z, p.Temperature, p.Luminosity));

                for (int i = 0; i < FillStarsPerPoint; i++)
                {
                    var ox = p.X + (float)(rng.NextDouble() - 0.5) * 2f * spread;
                    var oy = (p.Y + (float)(rng.NextDouble() - 0.5) * 2f * sectorRadius * 0.08f) * DiskThicknessScale;
                    var oz = p.Z + (float)(rng.NextDouble() - 0.5) * 2f * spread;

                    var tempShift = (float)(rng.NextDouble() - 0.5) * 2000f;
                    var temp = MathHelper.Clamp(p.Temperature + tempShift, 1000f, 10000f);
                    var lum = p.Luminosity * (FillLuminosityScale + (float)rng.NextDouble() * FillLuminosityScale);

                    expanded.Add(new GalaxyClusterPoint(ox, oy, oz, temp, lum));
                }
            }

            return expanded.ToArray();
        }

        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((GalaxySectorTexture)target);
        }
    }
}
