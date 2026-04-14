using FrameworkSDK.Common;
using Microsoft.Xna.Framework;
using X4World.Generation;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxySectorAggregatedData
    {
        public Color ClusterColor { get; }
        public float SectorRadius { get; }
        public float Inclination { get; }
        public float SpinAngle { get; }
        public GalaxyClusterPoint[] ClusterPoints { get; }
        public bool IsHalo { get; }

        public GalaxySectorAggregatedData(Color clusterColor, float sectorRadius,
            float inclination, float spinAngle, GalaxyClusterPoint[] clusterPoints,
            bool isHalo = false)
        {
            ClusterColor = clusterColor;
            SectorRadius = sectorRadius;
            Inclination = inclination;
            SpinAngle = spinAngle;
            ClusterPoints = clusterPoints;
            IsHalo = isHalo;
        }
    }

    public class GalaxySector : WrappedDetailsBase<GalaxySectorAggregatedData>
    {
        public GalaxySector(IWrappedDetails parent, Vector3 localPosition,
            GalaxySectorAggregatedData aggregatedData)
            : base(parent, localPosition, aggregatedData)
        {
            var cfg = GalaxyConfig.Instance.GalaxySector.Node;
            LayerName = "06_GalaxySector";
            Name = $"{parent.Name}_sec{NamesGenerator.Hash(HashType.Number)}";
            DistanceToUnwrapDetails = aggregatedData.SectorRadius * cfg.UnwrapDistanceMultiplier;
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, aggregatedData.SectorRadius * cfg.OctreeSizeMultiplier, 10);
        }
    }
}
