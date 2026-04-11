using System.Collections.Generic;
using Microsoft.Xna.Framework;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxySectorTextureDetailsGenerator : IDetailsGenerator<GalaxySectorTexture>
    {
        private const float DiskThicknessScale = 2.5f;

        public void Generate(GalaxySectorTexture target)
        {
            var aggData = target.AggregatedData;

            var points = ScaleThickness(aggData.ClusterPoints);

            var sectorData = new GalaxySectorAggregatedData(
                aggData.GalaxyColor, aggData.SectorRadius,
                aggData.Inclination, aggData.SpinAngle,
                points);

            var sector = new GalaxySector(target, Vector3.Zero, sectorData);
            target.SetGeneratedData(new List<IWrappedDetails> { sector });
        }

        private static GalaxyClusterPoint[] ScaleThickness(GalaxyClusterPoint[] originals)
        {
            var result = new GalaxyClusterPoint[originals.Length];
            for (int i = 0; i < originals.Length; i++)
            {
                var p = originals[i];
                result[i] = new GalaxyClusterPoint(
                    p.X, p.Y * DiskThicknessScale, p.Z, p.Temperature, p.Luminosity);
            }
            return result;
        }

        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((GalaxySectorTexture)target);
        }
    }
}
