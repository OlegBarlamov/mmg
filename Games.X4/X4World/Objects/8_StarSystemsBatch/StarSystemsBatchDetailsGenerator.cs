using System.Collections.Generic;
using Microsoft.Xna.Framework;
using X4World.Maps;

namespace X4World.Objects
{
    public class StarSystemsBatchDetailsGenerator : IDetailsGenerator<StarSystemsBatch>
    {
        public void Generate(StarSystemsBatch target)
        {
            var aggData = target.AggregatedData;
            var clusterPoints = aggData.ClusterPoints;
            var results = new List<IWrappedDetails>(clusterPoints.Length);

            var galaxyRotation = Matrix.CreateRotationX(aggData.Inclination)
                               * Matrix.CreateRotationY(aggData.SpinAngle);

            for (int i = 0; i < clusterPoints.Length; i++)
            {
                var p = clusterPoints[i];
                var localPos = Vector3.Transform(new Vector3(p.X, p.Y, p.Z), galaxyRotation);
                var starData = new StarSystemAsPointAggregatedData(p.Temperature, p.Luminosity, i);
                results.Add(new StarSystemAsPoint(target, localPos, starData));
            }

            target.SetGeneratedData(results);
        }

        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((StarSystemsBatch)target);
        }
    }
}
