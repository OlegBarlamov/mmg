using FrameworkSDK.Common;
using Microsoft.Xna.Framework;
using X4World.Generation;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxySectorTextureAggregatedData
    {
        public Color GalaxyColor { get; }
        public float SectorRadius { get; }
        public float DiskRadius { get; }
        public float SectorCenterX { get; }
        public float SectorCenterZ { get; }
        public float Inclination { get; }
        public float SpinAngle { get; }
        public int Seed { get; }
        public GalaxyClusterPoint[] ClusterPoints { get; }
        public GalaxyTextureData TextureData { get; } = new GalaxyTextureData();

        public GalaxySectorTextureAggregatedData(
            Color galaxyColor, float sectorRadius, float diskRadius,
            float sectorCenterX, float sectorCenterZ,
            float inclination, float spinAngle,
            int seed, GalaxyClusterPoint[] clusterPoints)
        {
            GalaxyColor = galaxyColor;
            SectorRadius = sectorRadius;
            DiskRadius = diskRadius;
            SectorCenterX = sectorCenterX;
            SectorCenterZ = sectorCenterZ;
            Inclination = inclination;
            SpinAngle = spinAngle;
            Seed = seed;
            ClusterPoints = clusterPoints;
        }
    }

    public class GalaxySectorTexture : WrappedDetailsBase<GalaxySectorTextureAggregatedData>
    {
        public GalaxySectorTexture(IWrappedDetails parent, Vector3 localPosition,
            GalaxySectorTextureAggregatedData aggregatedData)
            : base(parent, localPosition, aggregatedData)
        {
            var cfg = GalaxyConfig.Instance.GalaxySectorTexture;
            LayerName = "5_GalaxySectorTexture";
            Name = $"{parent.Name}_stx{NamesGenerator.Hash(HashType.Number)}";
            DistanceToUnwrapDetails = aggregatedData.DiskRadius * cfg.UnwrapDistanceMultiplier;
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, aggregatedData.SectorRadius * cfg.OctreeSizeMultiplier, 10);
        }
    }
}
