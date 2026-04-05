using System;
using Microsoft.Xna.Framework;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxyTextureFarthestDetailsGenerator : IDetailsGenerator<GalaxyTextureFarthest>
    {
        public void Generate(GalaxyTextureFarthest target)
        {
            var aggData = target.AggregatedData;
            var rng = new Random(aggData.Seed);

            var sectorSeed = rng.Next();
            var layerSeeds = new[] { rng.Next(), rng.Next(), rng.Next() };
            var layerStarCounts = new[] { 1500, 2000, 1500 };

            var layeredData = new GalaxyTextureLayeredAggregatedData(
                aggData.GalaxyColor,
                aggData.ArmCount,
                aggData.DiskRadius,
                aggData.Inclination,
                aggData.SpinAngle,
                sectorSeed,
                layerSeeds,
                layerStarCounts);

            var layered = new GalaxyTextureLayered(target, Vector3.Zero, layeredData);

            target.SetGeneratedData(new[] { layered });
        }

        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((GalaxyTextureFarthest)target);
        }
    }
}
