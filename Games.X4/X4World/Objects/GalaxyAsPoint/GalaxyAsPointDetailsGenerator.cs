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
            var armCount = 2 + (int)(power * 4);
            var diskRadius = 20f + power * 20f;
            var inclination = RandomService.NextFloat(0f, MathHelper.Pi);
            var spinAngle = RandomService.NextFloat(0f, MathHelper.TwoPi);
            var seed = RandomService.NextInteger(0, int.MaxValue);

            var aggregatedData = new GalaxyTextureFarthestAggregatedData(
                target.AggregatedData.Color,
                armCount, diskRadius, inclination, spinAngle, seed);
            var item = new GalaxyTextureFarthest(target, Vector3.Zero, aggregatedData);

            target.SetGeneratedData(new[] { item });
        }
        
        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((GalaxyAsPoint) target);
        }
    }
}
