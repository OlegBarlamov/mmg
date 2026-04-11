using System;
using FrameworkSDK.Common;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using X4World.Generation;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxyAsPointDetailsGenerator : IDetailsGenerator<GalaxyAsPoint>
    {
        public IRandomService RandomService { get; }

        public GalaxyAsPointDetailsGenerator([NotNull] IRandomService randomService)
        {
            RandomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
        }
        
        public void Generate(GalaxyAsPoint target)
        {
            var cfg = GalaxyConfig.Instance.GalaxyAsPoint.Generation;
            var power = target.AggregatedData.Power;
            var galaxyColor = target.AggregatedData.Color;
            var armCount = cfg.BaseArmCount + (int)(power * cfg.ArmCountPowerScale);
            var diskRadius = cfg.BaseDiskRadius + power * cfg.DiskRadiusPowerScale;
            var inclination = RandomService.NextFloat(0f, MathHelper.Pi);
            var spinAngle = RandomService.NextFloat(0f, MathHelper.TwoPi);
            var seed = RandomService.NextInteger(0, int.MaxValue);

            var clusterCount = cfg.BaseClusterCount + (int)(power * cfg.ClusterCountPowerScale);
            var clusterPoints = GenerateClusterPoints(cfg, armCount, diskRadius, power, galaxyColor, seed, clusterCount);

            var aggregatedData = new GalaxyTextureFarthestAggregatedData(
                galaxyColor,
                armCount, diskRadius, inclination, spinAngle, seed, clusterPoints);
            var item = new GalaxyTextureFarthest(target, Vector3.Zero, aggregatedData);

            target.SetGeneratedData(new[] { item });
        }

        private static GalaxyClusterPoint[] GenerateClusterPoints(
            GalaxyAsPointGenerationConfig cfg, int armCount, float diskRadius, float power, Color galaxyColor, int seed, int clusterCount)
        {
            var rng = new Random(seed);
            var clusters = new GalaxyClusterPoint[clusterCount];

            var galaxyTemp = TemperatureFromColor(galaxyColor);
            var scatterScale = MathHelper.Lerp(cfg.ScatterScaleMax, cfg.ScatterScaleMin, power);

            for (int i = 0; i < clusterCount; i++)
            {
                var arm = i % armCount;
                var armBaseAngle = arm * MathHelper.TwoPi / armCount;

                var radialFraction = (float)(rng.NextDouble() * rng.NextDouble());
                var r = radialFraction * diskRadius * cfg.RadialExtentFactor;
                var windAngle = radialFraction * MathHelper.TwoPi * cfg.SpiralWindFactor;
                var scatter = (float)(rng.NextDouble() - 0.5) * 2f * (1f + r * 0.05f) * scatterScale;

                var angle = armBaseAngle + windAngle;
                var x = r * (float)Math.Cos(angle) + scatter;
                var z = r * (float)Math.Sin(angle) + scatter;
                var y = (float)(rng.NextDouble() - 0.5) * 2f * diskRadius * cfg.DiskThicknessRatio;

                var baseTemp = 1000f + (float)rng.NextDouble() * 9000f;
                var temperature = MathHelper.Lerp(baseTemp, galaxyTemp, cfg.TemperatureBlend);
                var luminosity = power * cfg.BaseLuminosityScale + (float)rng.NextDouble() * (power * cfg.LuminosityPowerScale + cfg.LuminosityRandomBase);

                clusters[i] = new GalaxyClusterPoint(x, y, z, temperature, luminosity);
            }

            return clusters;
        }

        private static float TemperatureFromColor(Color color)
        {
            var r = color.R / 255f;
            var b = color.B / 255f;
            var ratio = MathHelper.Clamp(b / Math.Max(r, 0.01f), 0f, 2f);
            return 1000f + ratio * 4500f;
        }
        
        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((GalaxyAsPoint) target);
        }
    }
}
