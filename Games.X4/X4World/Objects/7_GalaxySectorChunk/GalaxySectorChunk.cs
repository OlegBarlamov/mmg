using System;
using FrameworkSDK.Common;
using Microsoft.Xna.Framework;
using X4World.Generation;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxySectorChunkAggregatedData
    {
        public float ChunkRadius { get; }
        public float CellRadius { get; }
        public float SectorRadius { get; }
        public float Inclination { get; }
        public float SpinAngle { get; }
        public GalaxyClusterPoint[] ClusterPoints { get; }

        public GalaxySectorChunkAggregatedData(float chunkRadius, float cellRadius, float sectorRadius,
            float inclination, float spinAngle, GalaxyClusterPoint[] clusterPoints)
        {
            ChunkRadius = chunkRadius;
            CellRadius = cellRadius;
            SectorRadius = sectorRadius;
            Inclination = inclination;
            SpinAngle = spinAngle;
            ClusterPoints = clusterPoints;
        }
    }

    public class GalaxySectorChunk : WrappedDetailsBase<GalaxySectorChunkAggregatedData>
    {
        public GalaxySectorChunk(IWrappedDetails parent, Vector3 localPosition,
            GalaxySectorChunkAggregatedData aggregatedData)
            : base(parent, localPosition, aggregatedData)
        {
            var cfg = GalaxyConfig.Instance.GalaxySectorChunk.Node;
            LayerName = "07_GalaxySectorChunk";
            Name = $"{parent.Name}_ch{NamesGenerator.Hash(HashType.Number)}";
            DistanceToUnwrapDetails = aggregatedData.SectorRadius * cfg.UnwrapDistanceMultiplier;
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, aggregatedData.ChunkRadius * cfg.OctreeSizeMultiplier, 10);
        }
    }
}
