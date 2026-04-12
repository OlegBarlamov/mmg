using FrameworkSDK.Common;
using Microsoft.Xna.Framework;
using X4World.Generation;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxyTextureLayeredAggregatedData
    {
        public Color GalaxyColor { get; }
        public int ArmCount { get; }
        public float DiskRadius { get; }
        public float Inclination { get; }
        public float SpinAngle { get; }
        public int Seed { get; }
        public GalaxySectorDefinition[] Sectors { get; }
        public GalaxyTextureData TextureData { get; } = new GalaxyTextureData();

        public GalaxyTextureLayeredAggregatedData(
            Color galaxyColor,
            int armCount,
            float diskRadius,
            float inclination,
            float spinAngle,
            int seed,
            GalaxySectorDefinition[] sectors)
        {
            GalaxyColor = galaxyColor;
            ArmCount = armCount;
            DiskRadius = diskRadius;
            Inclination = inclination;
            SpinAngle = spinAngle;
            Seed = seed;
            Sectors = sectors;
        }
    }

    public class GalaxyTextureLayered : WrappedDetailsBase<GalaxyTextureLayeredAggregatedData>
    {
        public GalaxyTextureLayered(IWrappedDetails parent, Vector3 localPosition,
            GalaxyTextureLayeredAggregatedData aggregatedData)
            : base(parent, localPosition, aggregatedData)
        {
            var cfg = GalaxyConfig.Instance.GalaxyTextureLayered.Node;
            LayerName = "04_GalaxyTextureLayered";
            Name = $"{parent.Name}_gtl{NamesGenerator.Hash(HashType.Number)}";
            DistanceToUnwrapDetails = aggregatedData.DiskRadius * cfg.UnwrapDistanceMultiplier;
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, aggregatedData.DiskRadius * cfg.OctreeSizeMultiplier, 10);
        }
    }
}
