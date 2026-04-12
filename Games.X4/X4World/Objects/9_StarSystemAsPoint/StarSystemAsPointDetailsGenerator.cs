using System.Collections.Generic;
using Microsoft.Xna.Framework;
using X4World.Maps;

namespace X4World.Objects
{
    public class StarSystemAsPointDetailsGenerator : IDetailsGenerator<StarSystemAsPoint>
    {
        public void Generate(StarSystemAsPoint target)
        {
            var agg = target.AggregatedData;
            var starData = new StarSystemAggregatedData(agg.Temperature, agg.Luminosity, agg.Seed);
            var lightPoint = new StarSystemAsLightPoint(target, Vector3.Zero, starData);
            target.SetGeneratedData(new List<IWrappedDetails> { lightPoint });
        }

        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((StarSystemAsPoint)target);
        }
    }
}
