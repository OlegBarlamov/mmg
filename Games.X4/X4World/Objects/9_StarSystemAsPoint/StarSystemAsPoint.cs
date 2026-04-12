using FrameworkSDK.Common;
using Microsoft.Xna.Framework;
using X4World.Generation;
using X4World.Maps;

namespace X4World.Objects
{
    public class StarSystemAsPointAggregatedData
    {
        public float Temperature { get; }
        public float Luminosity { get; }
        public Color Color { get; }
        public int Seed { get; }

        public StarSystemAsPointAggregatedData(float temperature, float luminosity, int seed)
        {
            Temperature = temperature;
            Luminosity = luminosity;
            Color = GalaxyAsPointAggregatedData.ColorFromTemperature(temperature);
            Seed = seed;
        }
    }

    public class StarSystemAsPoint : WrappedDetailsBase<StarSystemAsPointAggregatedData>
    {
        public StarSystemAsPoint(IWrappedDetails parent, Vector3 localPosition,
            StarSystemAsPointAggregatedData aggregatedData)
            : base(parent, localPosition, aggregatedData)
        {
            var cfg = GalaxyConfig.Instance.StarSystemAsPoint.Node;
            LayerName = "09_StarSystemAsPoint";
            Name = $"{parent.Name}_sp{NamesGenerator.Hash(HashType.Number)}";
            DistanceToUnwrapDetails = cfg.UnwrapDistance;
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, 1f, 10);
        }
    }
}
