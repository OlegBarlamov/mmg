using System.Collections.Generic;
using FrameworkSDK.Common;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
using X4World.Maps;

namespace X4World.Objects
{
    public class SectorDimStarFieldAggregatedData
    {
        public float SectorRadius { get; }
        public int StarCount { get; }
        public int Seed { get; }

        public SectorDimStarFieldAggregatedData(float sectorRadius, int starCount, int seed)
        {
            SectorRadius = sectorRadius;
            StarCount = starCount;
            Seed = seed;
        }
    }

    public class SectorDimStarField : IWrappedDetails
    {
        public Vector3 Position { get; private set; }
        public string Name { get; }
        public IWrappedDetails Parent { get; }

        object IWrappedDetails.AggregatedData => AggregatedData;

        public float DistanceToUnwrapDetails { get; } = float.MaxValue;

        public bool IsDetailsGenerated { get; private set; }
        public IObjectsSpace<Vector3, IWrappedDetails> Details { get; }

        public SectorDimStarFieldAggregatedData AggregatedData { get; }

        public SectorDimStarField(IWrappedDetails parent, Vector3 localPosition,
            SectorDimStarFieldAggregatedData aggregatedData)
        {
            Position = localPosition;
            Parent = parent;
            AggregatedData = aggregatedData;
            Name = $"{Parent.Name}_dsf{NamesGenerator.Hash(HashType.Number)}";
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
