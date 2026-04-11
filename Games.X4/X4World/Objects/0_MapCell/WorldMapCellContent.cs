using System;
using System.Collections.Generic;
using FrameworkSDK.MonoGame.Graphics.RenderableComponents.Models;
using FrameworkSDK.MonoGame.Resources;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions.DataStructures;
using NetExtensions.Geometry;
using X4World.Generation;
using X4World.Maps;

namespace X4World.Objects
{
    public class WorldMapCellContent : ViewModel3D, IWrappedDetails
    {
        public IWrappedDetails Parent { get; } = null;

        public override void SetPosition(Vector3 position)
        {
        }

        object IWrappedDetails.AggregatedData => WorldMapCellAggregatedData;

        public Vector3 Size { get; }
        public WorldMapCellAggregatedData WorldMapCellAggregatedData { get; }
        
        public Point3D MapPoint { get; }

        public float DistanceToUnwrapDetails { get; }
        
        public bool IsDetailsGenerated { get; private set; }
        public IObjectsSpace<Vector3, IWrappedDetails> Details { get; }

        public WorldMapCellContent(Point3D point, Vector3 position, float size, [NotNull] WorldMapCellAggregatedData worldMapCellAggregatedData)
        {
            MapPoint = point;
            Name = $"{point.X},{point.Y},{point.Z}";
            Position = position;
            Size = new Vector3(size);
            WorldMapCellAggregatedData = worldMapCellAggregatedData ?? throw new ArgumentNullException(nameof(worldMapCellAggregatedData));

            var cfg = GalaxyConfig.Instance.MapCell.Node;
            DistanceToUnwrapDetails = cfg.CellSize * cfg.UnwrapDistanceMultiplier;
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, size, cfg.OctreeDepth);
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
        public string LayerName { get; } = "0_MapCell";
    }
}