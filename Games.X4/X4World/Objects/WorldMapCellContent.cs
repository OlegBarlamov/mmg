using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
using NetExtensions.Geometry;
using X4World.Maps;

namespace X4World.Objects
{
    public class WorldMapCellAggregatedData
    {
        public byte[,,] SubstanceMap { get; }

        public WorldMapCellAggregatedData(byte[,,] data)
        {
            SubstanceMap = data;
        }
    }
    
    public class WorldMapCellContent : IWrappedDetails
    {
        public IWrappedDetails Parent { get; } = null;
        public Vector3 Position
        {
            get => _position;
            set
            {
                // Nothing
            } 
        }

        object IWrappedDetails.AggregatedData => WorldMapCellAggregatedData;

        public Vector3 Size { get; }
        public WorldMapCellAggregatedData WorldMapCellAggregatedData { get; }

        public float DistanceToUnwrapDetails { get; } = WorldConstants.WorldMapCellSize;
        
        public bool IsDetailsGenerated { get; private set; }
        public IObjectsSpace<Vector3, IWrappedDetails> Details { get; }
        
        private readonly Vector3 _position;

        public WorldMapCellContent(Point3D point, Vector3 position, float size, [NotNull] WorldMapCellAggregatedData worldMapCellAggregatedData)
        {
            Name = $"{point.X},{point.Y},{point.Z}";
            _position = position;
            Size = new Vector3(size);
            WorldMapCellAggregatedData = worldMapCellAggregatedData ?? throw new ArgumentNullException(nameof(worldMapCellAggregatedData));
            
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, size, 10);
        }
        
        public Vector3 GetWorldPosition()
        {
            return Position;
        }

        public void SetGeneratedData(IReadOnlyCollection<IWrappedDetails> objects)
        {
            IsDetailsGenerated = true;
            foreach (var wrappedDetail in objects)
            {
                Details.Add(wrappedDetail);
            }
        }

        public string Name { get; }
    }
}