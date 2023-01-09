using System;
using System.Collections.Generic;
using FrameworkSDK;
using FrameworkSDK.Common;
using FrameworkSDK.MonoGame.Basic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions;
using MonoGameExtensions.DataStructures;
using NetExtensions.Helpers;
using X4World.Maps;

namespace X4World.Objects
{
    public class PlanetSystemFarthestAggregatedData
    {
        public IReadOnlyCollection<Vector3> PointsPositions { get; }
        public StarAsPointAggregatedData StarData { get; }

        public PlanetSystemFarthestAggregatedData([NotNull] IReadOnlyCollection<Vector3> pointsPositions,
            [NotNull] StarAsPointAggregatedData starData)
        {
            PointsPositions = pointsPositions ?? throw new ArgumentNullException(nameof(pointsPositions));
            StarData = starData ?? throw new ArgumentNullException(nameof(starData));
        }
    }
    
    public class PlanetSystemFarthest : IWrappedDetails 
    {
        public Vector3 Position { get; set; }
        public IWrappedDetails Parent { get; }

        object IWrappedDetails.AggregatedData => AggregatedData;

        public float DistanceToUnwrapDetails { get; } = 0;
        
        public bool IsDetailsGenerated { get; private set; }
        public IObjectsSpace<Vector3, IWrappedDetails> Details { get; }
        
        public PlanetSystemFarthestAggregatedData AggregatedData { get; }
        

        public PlanetSystemFarthest(IWrappedDetails parent, Vector3 localPosition, PlanetSystemFarthestAggregatedData aggregatedData)
        {
            Position = localPosition;
            Parent = parent;
            AggregatedData = aggregatedData;
            Name = $"{Parent.Name}_ps{NamesGenerator.Hash(HashType.Number)}";
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, 1, 10);
        }
        
        public Vector3 GetWorldPosition()
        {
            return Parent.GetWorldPosition() + Position;
        }

        public void SetGeneratedData(IReadOnlyCollection<IWrappedDetails> objects)
        {
            
        }

        public string Name { get; }
    }
}