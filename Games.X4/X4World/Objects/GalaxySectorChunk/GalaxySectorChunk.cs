using System.Collections.Generic;
using FrameworkSDK.Common;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxySectorChunkAggregatedData
    {
        public float ChunkRadius { get; }
        public float Inclination { get; }
        public float SpinAngle { get; }
        public GalaxyClusterPoint[] ClusterPoints { get; }

        public GalaxySectorChunkAggregatedData(float chunkRadius,
            float inclination, float spinAngle, GalaxyClusterPoint[] clusterPoints)
        {
            ChunkRadius = chunkRadius;
            Inclination = inclination;
            SpinAngle = spinAngle;
            ClusterPoints = clusterPoints;
        }
    }

    public class GalaxySectorChunk : IWrappedDetails
    {
        public Vector3 Position { get; private set; }
        public string Name { get; }
        public IWrappedDetails Parent { get; }

        object IWrappedDetails.AggregatedData => AggregatedData;

        public float DistanceToUnwrapDetails { get; }

        public bool IsDetailsGenerated { get; private set; }
        public IObjectsSpace<Vector3, IWrappedDetails> Details { get; }

        public GalaxySectorChunkAggregatedData AggregatedData { get; }

        public GalaxySectorChunk(IWrappedDetails parent, Vector3 localPosition, float unwrapDistance,
            GalaxySectorChunkAggregatedData aggregatedData)
        {
            Position = localPosition;
            Parent = parent;
            AggregatedData = aggregatedData;
            DistanceToUnwrapDetails = unwrapDistance;
            Name = $"{Parent.Name}_ch{NamesGenerator.Hash(HashType.Number)}";
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, aggregatedData.ChunkRadius * 10f, 10);
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
