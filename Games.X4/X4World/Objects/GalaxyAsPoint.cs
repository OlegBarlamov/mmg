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
    public class GalaxyAsPointAggregatedData
    {
        public float Power { get; }
        public Color Color { get; }

        public GalaxyAsPointAggregatedData(float power)
        {
            Power = power;
        }
    }
    
    public class GalaxyAsPoint : IWrappedDetails 
    {
        public Vector3 Position { get; set; }
        public IWrappedDetails Parent { get; }

        object IWrappedDetails.AggregatedData => AggregatedData;

        public float DistanceToUnwrapDetails { get; } = 1000;
        
        public bool IsDetailsGenerated { get; private set; }
        public IObjectsSpace<Vector3, IWrappedDetails> Details { get; }
        
        public GalaxyAsPointAggregatedData AggregatedData { get; }
        

        public GalaxyAsPoint(IWrappedDetails parent, Vector3 localPosition, GalaxyAsPointAggregatedData aggregatedData)
        {
            Position = localPosition;
            Parent = parent;
            AggregatedData = aggregatedData;
            Name = $"{Parent.Name}_g{NamesGenerator.Hash(HashType.Number)}";
            Details = new OctreeBasedObjectsSpace(Vector3.Zero, 100, 10);
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

        public string Name { get; }
    }
}