using System.Collections.Generic;
using FrameworkSDK.Common;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxyTextureLayeredAggregatedData
    {
        public const int LayerCount = 3;

        public Color GalaxyColor { get; }
        public int ArmCount { get; }
        public float DiskRadius { get; }
        public float Inclination { get; }
        public float SpinAngle { get; }
        public int Seed { get; }
        public int[] LayerSeeds { get; }
        public int[] LayerStarCounts { get; }
        public GalaxyTextureData[] LayerTextures { get; }

        public GalaxyTextureLayeredAggregatedData(
            Color galaxyColor,
            int armCount,
            float diskRadius,
            float inclination,
            float spinAngle,
            int seed,
            int[] layerSeeds,
            int[] layerStarCounts)
        {
            GalaxyColor = galaxyColor;
            ArmCount = armCount;
            DiskRadius = diskRadius;
            Inclination = inclination;
            SpinAngle = spinAngle;
            Seed = seed;
            LayerSeeds = layerSeeds;
            LayerStarCounts = layerStarCounts;
            LayerTextures = new GalaxyTextureData[LayerCount];
            for (int i = 0; i < LayerCount; i++)
                LayerTextures[i] = new GalaxyTextureData();
        }
    }

    public class GalaxyTextureLayered : IWrappedDetails
    {
        public Vector3 Position { get; private set; }
        public string Name { get; }
        public IWrappedDetails Parent { get; }

        object IWrappedDetails.AggregatedData => AggregatedData;

        public float DistanceToUnwrapDetails { get; }

        public bool IsDetailsGenerated { get; private set; }
        public IObjectsSpace<Vector3, IWrappedDetails> Details { get; }

        public GalaxyTextureLayeredAggregatedData AggregatedData { get; }

        public GalaxyTextureLayered(IWrappedDetails parent, Vector3 localPosition,
            GalaxyTextureLayeredAggregatedData aggregatedData)
        {
            Position = localPosition;
            Parent = parent;
            AggregatedData = aggregatedData;
            Name = $"{Parent.Name}_gtl{NamesGenerator.Hash(HashType.Number)}";
            DistanceToUnwrapDetails = aggregatedData.DiskRadius * 1.25f;
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, aggregatedData.DiskRadius * 2f, 10);
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
