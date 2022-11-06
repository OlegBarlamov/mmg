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
    public class StarAsSphereAggregatedData
    {
        public float Power { get; }
        public Color Color { get; }

        public StarAsSphereAggregatedData(float power)
        {
            Power = power;
        }
    }
    
    public class StarAsSphere : IWrappedDetails 
    {
        public Vector3 Position { get; set; }
        public IWrappedDetails Parent { get; }

        object IWrappedDetails.AggregatedData => AggregatedData;

        public float DistanceToUnwrapDetails { get; } = 0;
        
        public bool IsDetailsGenerated { get; private set; }
        public IObjectsSpace<Vector3, IWrappedDetails> Details { get; }
        
        public StarAsSphereAggregatedData AggregatedData { get; }
        

        public StarAsSphere(IWrappedDetails parent, Vector3 localPosition, StarAsSphereAggregatedData aggregatedData)
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