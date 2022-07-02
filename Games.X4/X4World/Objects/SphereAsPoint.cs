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
    public class SphereAsPointAggregatedData
    {
        public float Power { get; }
        public Color Color { get; }

        public SphereAsPointAggregatedData(float power)
        {
            Power = power;
        }
    }
    
    public class SphereAsPoint : IWrappedDetails 
    {
        public Vector3 Position { get; set; }
        public IWrappedDetails Parent { get; }

        object IWrappedDetails.AggregatedData => AggregatedData;

        public float DistanceToUnwrapDetails { get; } = 0;
        
        public bool IsDetailsGenerated { get; private set; }
        public IObjectsSpace<Vector3, IWrappedDetails> Details { get; }
        
        public SphereAsPointAggregatedData AggregatedData { get; }
        

        public SphereAsPoint(IWrappedDetails parent, Vector3 localPosition, SphereAsPointAggregatedData aggregatedData)
        {
            Position = localPosition;
            Parent = parent;
            AggregatedData = aggregatedData;
            Name = $"{Parent.Name}_s{NamesGenerator.Hash(HashType.Number)}";
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