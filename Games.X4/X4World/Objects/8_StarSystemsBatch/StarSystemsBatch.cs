using System;
using FrameworkSDK.Common;
using Microsoft.Xna.Framework;
using X4World.Generation;
using X4World.Maps;

namespace X4World.Objects
{
    public class StarSystemsBatchAggregatedData
    {
        public float BatchRadius { get; }
        public float CellRadius { get; }
        public float SectorRadius { get; }
        public float Inclination { get; }
        public float SpinAngle { get; }
        public GalaxyClusterPoint[] ClusterPoints { get; }

        public StarSystemsBatchAggregatedData(float batchRadius, float cellRadius, float sectorRadius,
            float inclination, float spinAngle, GalaxyClusterPoint[] clusterPoints)
        {
            BatchRadius = batchRadius;
            CellRadius = cellRadius;
            SectorRadius = sectorRadius;
            Inclination = inclination;
            SpinAngle = spinAngle;
            ClusterPoints = clusterPoints;
        }
    }

    public class StarSystemsBatch : WrappedDetailsBase<StarSystemsBatchAggregatedData>
    {
        public StarSystemsBatch(IWrappedDetails parent, Vector3 localPosition,
            StarSystemsBatchAggregatedData aggregatedData)
            : base(parent, localPosition, aggregatedData)
        {
            var cfg = GalaxyConfig.Instance.StarSystemsBatch.Node;
            LayerName = "08_StarSystemsBatch";
            Name = $"{parent.Name}_sb{NamesGenerator.Hash(HashType.Number)}";
            DistanceToUnwrapDetails = aggregatedData.SectorRadius * cfg.UnwrapDistanceMultiplier;
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, ComputeOctreeSize(aggregatedData, cfg.OctreeSizeMultiplier), 10);
        }

        private static float ComputeOctreeSize(StarSystemsBatchAggregatedData data, float sizeMultiplier)
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
            return maxExtent * sizeMultiplier;
        }
    }
}
