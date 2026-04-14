using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using X4World.Generation;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxySectorTextureDetailsGenerator : IDetailsGenerator<GalaxySectorTexture>
    {

        public void Generate(GalaxySectorTexture target)
        {
            var aggData = target.AggregatedData;
            var genCfg = GalaxyConfig.Instance.GalaxySectorTexture.Generation;

            var points = ScaleThickness(aggData.ClusterPoints, genCfg.DiskThicknessScale);

            var sectorData = new GalaxySectorAggregatedData(
                aggData.GalaxyColor, aggData.SectorRadius,
                aggData.Inclination, aggData.SpinAngle,
                points);

            var results = new List<IWrappedDetails> { new GalaxySector(target, Vector3.Zero, sectorData) };

            if (aggData.HaloYCount > 0)
                GenerateHaloSectors(target, aggData, genCfg, results);

            target.SetGeneratedData(results);
        }

        private static void GenerateHaloSectors(
            GalaxySectorTexture parent,
            GalaxySectorTextureAggregatedData aggData,
            GalaxySectorTextureGenerationConfig cfg,
            List<IWrappedDetails> results)
        {
            var rng = new Random(aggData.Seed ^ 0x48414C4F);
            var sectorRadius = aggData.SectorRadius;
            var yStep = sectorRadius * cfg.HaloYOffset;

            var galaxyRotation = Matrix.CreateRotationX(aggData.Inclination)
                               * Matrix.CreateRotationY(aggData.SpinAngle);

            for (int layer = 0; layer <= aggData.HaloYCount; layer++)
            {
                var yBase = yStep * layer;
                var layerFraction = aggData.HaloYCount > 0 ? (float)layer / aggData.HaloYCount : 0f;
                var starCount = Math.Max(5, (int)(cfg.HaloStarsPerSector * (1f - layerFraction * 0.6f)));
                var lumScale = cfg.HaloLuminosityScale * (1f - layerFraction * 0.5f);

                if (layer == 0)
                {
                    var xJitter = (float)(rng.NextDouble() - 0.5) * 2f * sectorRadius * 0.3f;
                    var zJitter = (float)(rng.NextDouble() - 0.5) * 2f * sectorRadius * 0.3f;

                    var haloPoints = GenerateHaloStars(rng, starCount, lumScale, sectorRadius);
                    var haloData = new GalaxySectorAggregatedData(
                        aggData.GalaxyColor, sectorRadius,
                        aggData.Inclination, aggData.SpinAngle,
                        haloPoints, isHalo: true);

                    var localPos = Vector3.Transform(new Vector3(xJitter, 0f, zJitter), galaxyRotation);
                    results.Add(new GalaxySector(parent, localPos, haloData));
                }
                else
                {
                    for (int sign = -1; sign <= 1; sign += 2)
                    {
                        var xJitter = (float)(rng.NextDouble() - 0.5) * 2f * sectorRadius * 0.3f;
                        var zJitter = (float)(rng.NextDouble() - 0.5) * 2f * sectorRadius * 0.3f;

                        var haloPoints = GenerateHaloStars(rng, starCount, lumScale, sectorRadius);
                        var haloData = new GalaxySectorAggregatedData(
                            aggData.GalaxyColor, sectorRadius,
                            aggData.Inclination, aggData.SpinAngle,
                            haloPoints, isHalo: true);

                        var localPos = Vector3.Transform(
                            new Vector3(xJitter, sign * yBase, zJitter), galaxyRotation);
                        results.Add(new GalaxySector(parent, localPos, haloData));
                    }
                }
            }
        }

        private static GalaxyClusterPoint[] GenerateHaloStars(
            Random rng, int starCount, float luminosityScale, float sectorRadius)
        {
            var stars = new GalaxyClusterPoint[starCount];
            var sigma = sectorRadius * 1.0f;

            for (int i = 0; i < stars.Length; i++)
            {
                var x = (float)(GaussianRandom(rng) * sigma);
                var y = (float)(GaussianRandom(rng) * sigma);
                var z = (float)(GaussianRandom(rng) * sigma);

                var temperature = 2000f + (float)rng.NextDouble() * 6000f;
                var luminosity = ((float)rng.NextDouble() * 0.5f + 0.2f) * luminosityScale;

                stars[i] = new GalaxyClusterPoint(x, y, z, temperature, luminosity);
            }

            return stars;
        }

        private static double GaussianRandom(Random rng)
        {
            var u1 = 1.0 - rng.NextDouble();
            var u2 = rng.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
        }

        private static GalaxyClusterPoint[] ScaleThickness(GalaxyClusterPoint[] originals, float thicknessScale)
        {
            var result = new GalaxyClusterPoint[originals.Length];
            for (int i = 0; i < originals.Length; i++)
            {
                var p = originals[i];
                result[i] = new GalaxyClusterPoint(
                    p.X, p.Y * thicknessScale, p.Z, p.Temperature, p.Luminosity);
            }
            return result;
        }

        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((GalaxySectorTexture)target);
        }
    }
}
