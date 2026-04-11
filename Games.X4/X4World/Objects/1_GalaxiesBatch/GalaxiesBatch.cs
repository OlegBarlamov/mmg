using FrameworkSDK.Common;
using Microsoft.Xna.Framework;
using X4World.Generation;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxiesBatchAggregatedData
    {
        public float CellSize { get; }
        public WorldMapCellAggregatedData.GalaxyPointData[] GalaxyPoints { get; }

        public GalaxiesBatchAggregatedData(float cellSize,
            WorldMapCellAggregatedData.GalaxyPointData[] galaxyPoints)
        {
            CellSize = cellSize;
            GalaxyPoints = galaxyPoints;
        }
    }

    public class GalaxiesBatch : WrappedDetailsBase<GalaxiesBatchAggregatedData>
    {
        public GalaxiesBatch(IWrappedDetails parent, Vector3 localPosition,
            GalaxiesBatchAggregatedData aggregatedData)
            : base(parent, localPosition, aggregatedData)
        {
            var cfg = GalaxyConfig.Instance.GalaxiesBatch.Node;
            LayerName = "1_GalaxiesBatch";
            Name = $"{parent.Name}_gb{NamesGenerator.Hash(HashType.Number)}";
            DistanceToUnwrapDetails = aggregatedData.CellSize * cfg.UnwrapDistanceMultiplier;
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, aggregatedData.CellSize * cfg.OctreeSizeMultiplier, 10);
        }
    }
}
