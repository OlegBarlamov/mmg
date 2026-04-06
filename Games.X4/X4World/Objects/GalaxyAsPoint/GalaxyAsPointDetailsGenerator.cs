using System;
using FrameworkSDK.Common;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
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
            var power = target.AggregatedData.Power;
            var galaxyColor = target.AggregatedData.Color;
            var armCount = 2 + (int)(power * 4);
            var diskRadius = 10f + power * 20f;
            var inclination = RandomService.NextFloat(0f, MathHelper.Pi);
            var spinAngle = RandomService.NextFloat(0f, MathHelper.TwoPi);
            var seed = RandomService.NextInteger(0, int.MaxValue);

            var clusterCount = 100 + (int)(power * 400);
            var clusterPoints = GenerateClusterPoints(armCount, diskRadius, power, galaxyColor, seed, clusterCount);

            var aggregatedData = new GalaxyTextureFarthestAggregatedData(
                galaxyColor,
                armCount, diskRadius, inclination, spinAngle, seed, clusterPoints);
            var item = new GalaxyTextureFarthest(target, Vector3.Zero, aggregatedData);

            target.SetGeneratedData(new[] { item });
        }

        private static GalaxyClusterPoint[] GenerateClusterPoints(
            int armCount, float diskRadius, float power, Color galaxyColor, int seed, int clusterCount)
        {
            var rng = new Random(seed);
            var clusters = new GalaxyClusterPoint[clusterCount];

            var galaxyTemp = TemperatureFromColor(galaxyColor);
            var scatterScale = MathHelper.Lerp(1.5f, 0.5f, power);

            for (int i = 0; i < clusterCount; i++)
            {
                var arm = i % armCount;
                var armBaseAngle = arm * MathHelper.TwoPi / armCount;

                var radialFraction = (float)(rng.NextDouble() * rng.NextDouble());
                var r = radialFraction * diskRadius * 0.9f;
                var windAngle = radialFraction * MathHelper.TwoPi * 1.5f;
                var scatter = (float)(rng.NextDouble() - 0.5) * 2f * (1f + r * 0.05f) * scatterScale;

                var angle = armBaseAngle + windAngle;
                var x = r * (float)Math.Cos(angle) + scatter;
                var z = r * (float)Math.Sin(angle) + scatter;
                var y = (float)(rng.NextDouble() - 0.5) * 2f * diskRadius * 0.05f;

                var baseTemp = 1000f + (float)rng.NextDouble() * 9000f;
                var temperature = MathHelper.Lerp(baseTemp, galaxyTemp, 0.4f);
                var luminosity = power * 0.3f + (float)rng.NextDouble() * (power * 2f + 0.5f);

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
