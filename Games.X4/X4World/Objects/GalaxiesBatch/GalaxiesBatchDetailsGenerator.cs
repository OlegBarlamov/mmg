using System.Collections.Generic;
using Microsoft.Xna.Framework;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxiesBatchDetailsGenerator : IDetailsGenerator<GalaxiesBatch>
    {
        public void Generate(GalaxiesBatch target)
        {
            var aggData = target.AggregatedData;
            var galaxyPoints = aggData.GalaxyPoints;
            var results = new List<IWrappedDetails>(galaxyPoints.Length);

            for (int i = 0; i < galaxyPoints.Length; i++)
            {
                var gp = galaxyPoints[i];
                var color = GalaxyAsPointAggregatedData.ColorFromTemperature(gp.Temperature);
                var itemAggregatedData = new GalaxyAsPointAggregatedData(gp.Power, color);
                var galaxy = new GalaxyAsPoint(target, gp.LocalPositionFromCenter, itemAggregatedData);
                results.Add(galaxy);
            }

            target.SetGeneratedData(results);
        }

        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((GalaxiesBatch)target);
        }
    }
}
