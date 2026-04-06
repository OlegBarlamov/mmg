using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxyTextureLayeredDetailsGenerator : IDetailsGenerator<GalaxyTextureLayered>
    {
        public void Generate(GalaxyTextureLayered target)
        {
            var aggData = target.AggregatedData;
            var sectors = aggData.Sectors;
            var results = new List<IWrappedDetails>(sectors.Length);

            var galaxyRotation = Matrix.CreateRotationX(aggData.Inclination)
                               * Matrix.CreateRotationY(aggData.SpinAngle);

            var unwrapDistance = aggData.DiskRadius * 1.5f;

            for (int i = 0; i < sectors.Length; i++)
            {
                var sector = sectors[i];
                var localPos = Vector3.Transform(new Vector3(sector.CenterX, 0f, sector.CenterZ), galaxyRotation);

                var sectorTextureData = new GalaxySectorTextureAggregatedData(
                    aggData.GalaxyColor, sector.Radius, aggData.DiskRadius,
                    sector.CenterX, sector.CenterZ,
                    aggData.Inclination, aggData.SpinAngle,
                    sector.Seed, sector.ClusterPoints);
                var sectorTexture = new GalaxySectorTexture(target, localPos, unwrapDistance, sectorTextureData);
                results.Add(sectorTexture);
            }

            target.SetGeneratedData(results);
        }

        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((GalaxyTextureLayered)target);
        }
    }
}
