using System;
using System.Collections.Generic;
using FrameworkSDK.Common;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
using X4World.Maps;

namespace X4World.Objects
{
    public class GalaxiesBatchAggregatedData
    {
        public float CellSize { get; }
        public WorldMapCellAggregatedData.GalaxyPointData[] GalaxyPoints { get; }

        public GalaxiesBatchAggregatedData(float cellSize,
            WorldMapCellAggregatedData.GalaxyPointData[] galaxyPoints)
        {
            CellSize = cellSize;
            GalaxyPoints = galaxyPoints;
        }
    }

    public class GalaxiesBatch : IWrappedDetails
    {
        public Vector3 Position { get; private set; }
        public string Name { get; }
        public IWrappedDetails Parent { get; }

        object IWrappedDetails.AggregatedData => AggregatedData;

        public float DistanceToUnwrapDetails { get; }

        public bool IsDetailsGenerated { get; private set; }
        public IObjectsSpace<Vector3, IWrappedDetails> Details { get; }

        public GalaxiesBatchAggregatedData AggregatedData { get; }

        public GalaxiesBatch(IWrappedDetails parent, Vector3 localPosition, float unwrapDistance,
            GalaxiesBatchAggregatedData aggregatedData)
        {
            Position = localPosition;
            Parent = parent;
            AggregatedData = aggregatedData;
            DistanceToUnwrapDetails = unwrapDistance;
            Name = $"{Parent.Name}_gb{NamesGenerator.Hash(HashType.Number)}";
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, aggregatedData.CellSize * 1.5f, 10);
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
