using System.Collections.Generic;
using FrameworkSDK.Common;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
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
    
    public class GalaxyTextureFarthest : IWrappedDetails 
    {
        public Vector3 Position { get; private set; }
        public string Name { get; }
        public IWrappedDetails Parent { get; }

        object IWrappedDetails.AggregatedData => AggregatedData;

        public float DistanceToUnwrapDetails { get; }
        
        public bool IsDetailsGenerated { get; private set; }
        public IObjectsSpace<Vector3, IWrappedDetails> Details { get; }
        
        public GalaxyTextureFarthestAggregatedData AggregatedData { get; }

        public GalaxyTextureFarthest(IWrappedDetails parent, Vector3 localPosition, GalaxyTextureFarthestAggregatedData aggregatedData)
        {
            Position = localPosition;
            Parent = parent;
            AggregatedData = aggregatedData;
            Name = $"{Parent.Name}_gt{NamesGenerator.Hash(HashType.Number)}";
            DistanceToUnwrapDetails = aggregatedData.DiskRadius * 7f;
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, aggregatedData.DiskRadius * 2f, 10);
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
