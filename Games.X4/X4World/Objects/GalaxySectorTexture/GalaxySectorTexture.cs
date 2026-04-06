using System.Collections.Generic;
using FrameworkSDK.Common;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
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

    public class GalaxySectorTexture : IWrappedDetails
    {
        public Vector3 Position { get; private set; }
        public string Name { get; }
        public IWrappedDetails Parent { get; }

        object IWrappedDetails.AggregatedData => AggregatedData;

        public float DistanceToUnwrapDetails { get; }

        public bool IsDetailsGenerated { get; private set; }
        public IObjectsSpace<Vector3, IWrappedDetails> Details { get; }

        public GalaxySectorTextureAggregatedData AggregatedData { get; }

        public GalaxySectorTexture(IWrappedDetails parent, Vector3 localPosition, float unwrapDistance, GalaxySectorTextureAggregatedData aggregatedData)
        {
            Position = localPosition;
            Parent = parent;
            AggregatedData = aggregatedData;
            DistanceToUnwrapDetails = unwrapDistance;
            Name = $"{Parent.Name}_stx{NamesGenerator.Hash(HashType.Number)}";
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, aggregatedData.SectorRadius * 2f, 10);
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
