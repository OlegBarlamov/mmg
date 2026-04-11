using System.Collections.Generic;
using FrameworkSDK.Common;
using Microsoft.Xna.Framework;
using X4World.Maps;

namespace X4World.Objects
{
    public class StarSystemAggregatedData
    {
        public float Temperature { get; }
        public float Luminosity { get; }
        public Color Color { get; }
        public int Seed { get; }

        public StarSystemAggregatedData(float temperature, float luminosity, int seed)
        {
            Temperature = temperature;
            Luminosity = luminosity;
            Color = GalaxyAsPointAggregatedData.ColorFromTemperature(temperature);
            Seed = seed;
        }
    }

    public class StarSystemAsPoint : WrappedDetailsBase<StarSystemAggregatedData>
    {
        public StarSystemAsPoint(IWrappedDetails parent, Vector3 localPosition,
            StarSystemAggregatedData aggregatedData)
            : base(parent, localPosition, aggregatedData)
        {
            LayerName = "9_StarSystemAsPoint";
            Name = $"{parent.Name}_ss{NamesGenerator.Hash(HashType.Number)}";
            DistanceToUnwrapDetails = 0f;
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, 1f, 10);
        }

        public override void SetGeneratedData(IReadOnlyCollection<IWrappedDetails> objects)
        {
        }
    }
}
