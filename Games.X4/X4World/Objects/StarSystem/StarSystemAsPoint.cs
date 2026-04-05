using System.Collections.Generic;
using FrameworkSDK.Common;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
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

    public class StarSystemAsPoint : IWrappedDetails
    {
        public Vector3 Position { get; private set; }
        public string Name { get; }
        public IWrappedDetails Parent { get; }

        object IWrappedDetails.AggregatedData => AggregatedData;

        public float DistanceToUnwrapDetails { get; } = 0;

        public bool IsDetailsGenerated { get; private set; }
        public IObjectsSpace<Vector3, IWrappedDetails> Details { get; }

        public StarSystemAggregatedData AggregatedData { get; }

        public StarSystemAsPoint(IWrappedDetails parent, Vector3 localPosition, StarSystemAggregatedData aggregatedData)
        {
            Position = localPosition;
            Parent = parent;
            AggregatedData = aggregatedData;
            Name = $"{Parent.Name}_ss{NamesGenerator.Hash(HashType.Number)}";
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, 1, 10);
        }

        public void SetPosition(Vector3 position)
        {
            Position = position;
        }

        public Vector3 GetWorldPosition()
        {
            return Parent.GetWorldPosition() + Position;
        }

        public void SetGeneratedData(IReadOnlyCollection<IWrappedDetails> objects)
        {
        }
    }
}
