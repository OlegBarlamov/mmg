using System.Collections.Generic;
using FrameworkSDK.Common;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxySectorAggregatedData
    {
        public Color GalaxyColor { get; }
        public int ArmCount { get; }
        public float SectorRadius { get; }
        public int StarCount { get; }
        public int Seed { get; }

        public GalaxySectorAggregatedData(Color galaxyColor, int armCount, float sectorRadius, int starCount, int seed)
        {
            GalaxyColor = galaxyColor;
            ArmCount = armCount;
            SectorRadius = sectorRadius;
            StarCount = starCount;
            Seed = seed;
        }
    }

    public class GalaxySector : IWrappedDetails
    {
        public Vector3 Position { get; private set; }
        public string Name { get; }
        public IWrappedDetails Parent { get; }

        object IWrappedDetails.AggregatedData => AggregatedData;

        public float DistanceToUnwrapDetails { get; }

        public bool IsDetailsGenerated { get; private set; }
        public IObjectsSpace<Vector3, IWrappedDetails> Details { get; }

        public GalaxySectorAggregatedData AggregatedData { get; }

        public GalaxySector(IWrappedDetails parent, Vector3 localPosition, float unwrapDistance, GalaxySectorAggregatedData aggregatedData)
        {
            Position = localPosition;
            Parent = parent;
            AggregatedData = aggregatedData;
            DistanceToUnwrapDetails = unwrapDistance;
            Name = $"{Parent.Name}_sec{NamesGenerator.Hash(HashType.Number)}";
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
