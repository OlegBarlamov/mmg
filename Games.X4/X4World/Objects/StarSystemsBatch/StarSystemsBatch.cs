using System;
using System.Collections.Generic;
using FrameworkSDK.Common;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
using X4World.Maps;

namespace X4World.Objects
{
    public class StarSystemsBatchAggregatedData
    {
        public float BatchRadius { get; }
        public float Inclination { get; }
        public float SpinAngle { get; }
        public GalaxyClusterPoint[] ClusterPoints { get; }

        public StarSystemsBatchAggregatedData(float batchRadius,
            float inclination, float spinAngle, GalaxyClusterPoint[] clusterPoints)
        {
            BatchRadius = batchRadius;
            Inclination = inclination;
            SpinAngle = spinAngle;
            ClusterPoints = clusterPoints;
        }
    }

    public class StarSystemsBatch : IWrappedDetails
    {
        public Vector3 Position { get; private set; }
        public string Name { get; }
        public IWrappedDetails Parent { get; }

        object IWrappedDetails.AggregatedData => AggregatedData;

        public float DistanceToUnwrapDetails { get; }

        public bool IsDetailsGenerated { get; private set; }
        public IObjectsSpace<Vector3, IWrappedDetails> Details { get; }

        public StarSystemsBatchAggregatedData AggregatedData { get; }

        public StarSystemsBatch(IWrappedDetails parent, Vector3 localPosition, float unwrapDistance,
            StarSystemsBatchAggregatedData aggregatedData)
        {
            Position = localPosition;
            Parent = parent;
            AggregatedData = aggregatedData;
            DistanceToUnwrapDetails = unwrapDistance;
            Name = $"{Parent.Name}_sb{NamesGenerator.Hash(HashType.Number)}";
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, ComputeOctreeSize(aggregatedData), 10);
        }

        private static float ComputeOctreeSize(StarSystemsBatchAggregatedData data)
        {
            var rotation = Matrix.CreateRotationX(data.Inclination)
                         * Matrix.CreateRotationY(data.SpinAngle);
            float maxExtent = data.BatchRadius;
            foreach (var p in data.ClusterPoints)
            {
                var v = Vector3.Transform(new Vector3(p.X, p.Y, p.Z), rotation);
                maxExtent = Math.Max(maxExtent, Math.Abs(v.X));
                maxExtent = Math.Max(maxExtent, Math.Abs(v.Y));
                maxExtent = Math.Max(maxExtent, Math.Abs(v.Z));
            }
            return maxExtent * 2.5f;
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
