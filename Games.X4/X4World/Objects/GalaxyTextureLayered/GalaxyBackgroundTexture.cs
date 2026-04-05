using System.Collections.Generic;
using FrameworkSDK.Common;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxyBackgroundTextureAggregatedData
    {
        public Color GalaxyColor { get; }
        public int ArmCount { get; }
        public float DiskRadius { get; }
        public float Inclination { get; }
        public float SpinAngle { get; }
        public int Seed { get; }
        public GalaxyTextureData TextureData { get; } = new GalaxyTextureData();

        public GalaxyBackgroundTextureAggregatedData(
            Color galaxyColor,
            int armCount,
            float diskRadius,
            float inclination,
            float spinAngle,
            int seed)
        {
            GalaxyColor = galaxyColor;
            ArmCount = armCount;
            DiskRadius = diskRadius;
            Inclination = inclination;
            SpinAngle = spinAngle;
            Seed = seed;
        }
    }

    public class GalaxyBackgroundTexture : IWrappedDetails
    {
        public Vector3 Position { get; private set; }
        public string Name { get; }
        public IWrappedDetails Parent { get; }

        object IWrappedDetails.AggregatedData => AggregatedData;

        public float DistanceToUnwrapDetails { get; }

        public bool IsDetailsGenerated { get; private set; } = true;
        public IObjectsSpace<Vector3, IWrappedDetails> Details { get; }

        public GalaxyBackgroundTextureAggregatedData AggregatedData { get; }

        public GalaxyBackgroundTexture(IWrappedDetails parent, Vector3 localPosition,
            GalaxyBackgroundTextureAggregatedData aggregatedData)
        {
            Position = localPosition;
            Parent = parent;
            AggregatedData = aggregatedData;
            Name = $"{Parent.Name}_gbg{NamesGenerator.Hash(HashType.Number)}";
            DistanceToUnwrapDetails = aggregatedData.DiskRadius * 0.5f;
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, 1f, 1);
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
        }
    }
}
