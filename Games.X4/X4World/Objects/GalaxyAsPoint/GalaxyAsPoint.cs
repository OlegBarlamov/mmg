using System.Collections.Generic;
using FrameworkSDK.Common;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxyAsPointAggregatedData
    {
        public float Power { get; }
        public Color Color { get; }

        public GalaxyAsPointAggregatedData(float power, Color color)
        {
            Power = power;
            Color = color;
        }

        public static Color ColorFromTemperature(float temperature)
        {
            var t = MathHelper.Clamp((temperature - 1000f) / 9000f, 0f, 1f);
            if (t < 0.3f)
            {
                var s = t / 0.3f;
                return new Color(1.0f, 0.4f + 0.4f * s, 0.2f * s);
            }
            if (t < 0.6f)
            {
                var s = (t - 0.3f) / 0.3f;
                return new Color(1.0f, 0.8f + 0.2f * s, 0.2f + 0.8f * s);
            }
            var u = (t - 0.6f) / 0.4f;
            return new Color(0.7f + 0.3f * (1f - u), 0.8f + 0.2f * (1f - u), 1.0f);
        }
    }
    
    public class GalaxyAsPoint : IWrappedDetails 
    {
        public Vector3 Position { get; private set; }
        public string Name { get; }
        public IWrappedDetails Parent { get; }

        object IWrappedDetails.AggregatedData => AggregatedData;

        public float DistanceToUnwrapDetails { get; } = 2000;
        
        public bool IsDetailsGenerated { get; private set; }
        public IObjectsSpace<Vector3, IWrappedDetails> Details { get; }
        
        public GalaxyAsPointAggregatedData AggregatedData { get; }

        public GalaxyAsPoint(IWrappedDetails parent, Vector3 localPosition, GalaxyAsPointAggregatedData aggregatedData)
        {
            Position = localPosition;
            Parent = parent;
            AggregatedData = aggregatedData;
            Name = $"{Parent.Name}_g{NamesGenerator.Hash(HashType.Number)}";
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, 100, 10);
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
            IsDetailsGenerated = true;
            foreach (var wrappedDetail in objects)
            {
                Details.Add(wrappedDetail);
            }
        }
    }
}
