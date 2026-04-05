using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxyTextureLayeredDetailsGenerator : IDetailsGenerator<GalaxyTextureLayered>
    {
        private const int SectorCount = 30;
        private const int StarsPerSector = 100;
        private const float DiskThickness = 0.05f;

        public void Generate(GalaxyTextureLayered target)
        {
            var aggData = target.AggregatedData;
            var rng = new Random(aggData.Seed);
            var results = new List<IWrappedDetails>(SectorCount + 1);

            var backgroundData = new GalaxyBackgroundTextureAggregatedData(
                aggData.GalaxyColor, aggData.ArmCount, aggData.DiskRadius,
                aggData.Inclination, aggData.SpinAngle, rng.Next());
            results.Add(new GalaxyBackgroundTexture(target, Vector3.Zero, backgroundData));

            var galaxyRotation = Matrix.CreateRotationX(aggData.Inclination)
                               * Matrix.CreateRotationY(aggData.SpinAngle);

            var sectorRadius = aggData.DiskRadius / 6f;
            var unwrapDistance = sectorRadius * 1.5f;

            for (int i = 0; i < SectorCount; i++)
            {
                var arm = i % aggData.ArmCount;
                var armBaseAngle = arm * MathHelper.TwoPi / aggData.ArmCount;

                var radialFraction = (float)(rng.NextDouble() * rng.NextDouble());
                var r = radialFraction * aggData.DiskRadius * 0.9f;
                var windAngle = radialFraction * MathHelper.TwoPi * 1.5f;
                var scatter = (float)(rng.NextDouble() - 0.5) * 2f * (1f + r * 0.05f);

                var angle = armBaseAngle + windAngle;
                var x = r * (float)Math.Cos(angle) + scatter;
                var z = r * (float)Math.Sin(angle) + scatter;
                var y = (float)(rng.NextDouble() - 0.5) * 2f * aggData.DiskRadius * DiskThickness;

                var localPos = Vector3.Transform(new Vector3(x, y, z), galaxyRotation);
                var seed = rng.Next();

                var sectorData = new GalaxySectorAggregatedData(
                    aggData.GalaxyColor, aggData.ArmCount, sectorRadius, StarsPerSector, seed);
                var sector = new GalaxySector(target, localPos, unwrapDistance, sectorData);
                results.Add(sector);
            }

            target.SetGeneratedData(results);
        }

        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((GalaxyTextureLayered)target);
        }
    }
}
