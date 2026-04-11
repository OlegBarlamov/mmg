using FrameworkSDK.Common;
using Microsoft.Xna.Framework;
using X4World.Generation;
using X4World.Maps;

namespace X4World.Objects
{
    public struct GalaxyClusterPoint
    {
        public float X;
        public float Y;
        public float Z;
        public float Temperature;
        public float Luminosity;

        public GalaxyClusterPoint(float x, float y, float z, float temperature, float luminosity)
        {
            X = x;
            Y = y;
            Z = z;
            Temperature = temperature;
            Luminosity = luminosity;
        }
    }

    public class GalaxySectorDefinition
    {
        public float CenterX { get; }
        public float CenterZ { get; }
        public float Radius { get; }
        public int Seed { get; }
        public GalaxyClusterPoint[] ClusterPoints { get; }

        public GalaxySectorDefinition(float centerX, float centerZ, float radius, int seed, GalaxyClusterPoint[] clusterPoints)
        {
            CenterX = centerX;
            CenterZ = centerZ;
            Radius = radius;
            Seed = seed;
            ClusterPoints = clusterPoints;
        }
    }

    public class GalaxyTextureFarthestAggregatedData
    {
        public Color GalaxyColor { get; }
        public int ArmCount { get; }
        public float DiskRadius { get; }
        public float Inclination { get; }
        public float SpinAngle { get; }
        public int Seed { get; }
        public GalaxyClusterPoint[] ClusterPoints { get; }
        public GalaxyTextureData TextureData { get; } = new GalaxyTextureData();

        public GalaxyTextureFarthestAggregatedData(
            Color galaxyColor,
            int armCount,
            float diskRadius,
            float inclination,
            float spinAngle,
            int seed,
            GalaxyClusterPoint[] brightStars)
        {
            GalaxyColor = galaxyColor;
            ArmCount = armCount;
            DiskRadius = diskRadius;
            Inclination = inclination;
            SpinAngle = spinAngle;
            Seed = seed;
            ClusterPoints = brightStars;
        }
    }
    
    public class GalaxyTextureFarthest : WrappedDetailsBase<GalaxyTextureFarthestAggregatedData>
    {
        public GalaxyTextureFarthest(IWrappedDetails parent, Vector3 localPosition,
            GalaxyTextureFarthestAggregatedData aggregatedData)
            : base(parent, localPosition, aggregatedData)
        {
            var cfg = GalaxyConfig.Instance.GalaxyTextureFarthest.Node;
            LayerName = "3_GalaxyTextureFarthest";
            Name = $"{parent.Name}_gt{NamesGenerator.Hash(HashType.Number)}";
            DistanceToUnwrapDetails = aggregatedData.DiskRadius * cfg.UnwrapDistanceMultiplier;
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, aggregatedData.DiskRadius * cfg.OctreeSizeMultiplier, 10);
        }
    }
}
