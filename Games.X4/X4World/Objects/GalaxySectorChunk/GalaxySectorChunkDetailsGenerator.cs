using System.Collections.Generic;
using Microsoft.Xna.Framework;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxySectorChunkDetailsGenerator : IDetailsGenerator<GalaxySectorChunk>
    {
        public void Generate(GalaxySectorChunk target)
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

                var starData = new StarSystemAggregatedData(p.Temperature, p.Luminosity, i);
                var starSystem = new StarSystemAsPoint(target, localPos, starData);
                results.Add(starSystem);
            }

            target.SetGeneratedData(results);
        }

        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((GalaxySectorChunk)target);
        }
    }
}
