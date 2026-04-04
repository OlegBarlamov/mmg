using System;
using System.Collections.Generic;
using FrameworkSDK.Common;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxyTextureFarthestAggregatedData
    {
        public IReadOnlyCollection<Vector3> PointsPositions { get; }
        public GalaxyAsPointAggregatedData GalaxyData { get; }

        public GalaxyTextureFarthestAggregatedData([NotNull] IReadOnlyCollection<Vector3> pointsPositions,
            [NotNull] GalaxyAsPointAggregatedData galaxyData)
        {
            PointsPositions = pointsPositions ?? throw new ArgumentNullException(nameof(pointsPositions));
            GalaxyData = galaxyData ?? throw new ArgumentNullException(nameof(galaxyData));
        }
    }
    
    public class GalaxyTextureFarthest : IWrappedDetails 
    {
        public Vector3 Position { get; private set; }
        public string Name { get; }
        public IWrappedDetails Parent { get; }

        object IWrappedDetails.AggregatedData => AggregatedData;

        public float DistanceToUnwrapDetails { get; } = 0;
        
        public bool IsDetailsGenerated { get; private set; }
        public IObjectsSpace<Vector3, IWrappedDetails> Details { get; }
        
        public GalaxyTextureFarthestAggregatedData AggregatedData { get; }

        public GalaxyTextureFarthest(IWrappedDetails parent, Vector3 localPosition, GalaxyTextureFarthestAggregatedData aggregatedData)
        {
            Position = localPosition;
            Parent = parent;
            AggregatedData = aggregatedData;
            Name = $"{Parent.Name}_gt{NamesGenerator.Hash(HashType.Number)}";
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, 1, 10);
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
        }
    }
}