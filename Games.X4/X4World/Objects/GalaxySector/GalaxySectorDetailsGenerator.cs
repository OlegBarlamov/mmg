using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxySectorDetailsGenerator : IDetailsGenerator<GalaxySector>
    {
        private const int DimStarCount = 300;

        public void Generate(GalaxySector target)
        {
            var aggData = target.AggregatedData;
            var rng = new Random(aggData.Seed);
            var results = new List<IWrappedDetails>(aggData.StarCount + 1);

            var dimFieldData = new SectorDimStarFieldAggregatedData(
                aggData.SectorRadius, DimStarCount, rng.Next());
            results.Add(new SectorDimStarField(target, Vector3.Zero, dimFieldData));

            for (int i = 0; i < aggData.StarCount; i++)
            {
                var offsetX = (float)(rng.NextDouble() - 0.5) * 2f * aggData.SectorRadius;
                var offsetY = (float)(rng.NextDouble() - 0.5) * 2f * aggData.SectorRadius * 0.1f;
                var offsetZ = (float)(rng.NextDouble() - 0.5) * 2f * aggData.SectorRadius;
                var localPos = new Vector3(offsetX, offsetY, offsetZ);

                var temperature = 1000f + (float)rng.NextDouble() * 9000f;
                var luminosity = 0.3f + (float)rng.NextDouble() * 2.7f;
                var seed = rng.Next();

                var starData = new StarSystemAggregatedData(temperature, luminosity, seed);
                var starSystem = new StarSystemAsPoint(target, localPos, starData);
                results.Add(starSystem);
            }

            target.SetGeneratedData(results);
        }

        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((GalaxySector)target);
        }
    }
}
