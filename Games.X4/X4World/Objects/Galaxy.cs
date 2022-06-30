using System;
using System.Collections.Generic;
using FrameworkSDK;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions;
using MonoGameExtensions.DataStructures;
using NetExtensions.Helpers;
using X4World.Maps;

namespace X4World.Objects
{
    public class Galaxy : IGalaxiesLevelObject, INamed
    {
        public string Name { get; }
        public Vector3 Position { get; set; }
        public Vector3 Size { get; }
        public GlobalWorldMapCell OwnedCell { get; }
        
        public bool IsGenerated { get; }
        
        // All data which are needed to generate content
        public float SubstanceAmount { get; }
        
        
        public AutoSplitOctreeNode<Star> StarsOctree { get; }

        public Galaxy([NotNull] GlobalWorldMapCell cell, Vector3 worldPosition, string name)
        {
            OwnedCell = cell ?? throw new ArgumentNullException(nameof(cell));
            Position = worldPosition;
            Name = name;

            StarsOctree = new AutoSplitOctreeNode<Star>(Vector3.Zero, MathExtended.Max(Size.X, Size.Y, Size.Z), 10);
        }

        public void AddStar([NotNull] Star star)
        {
            if (star == null) throw new ArgumentNullException(nameof(star));
            
            StarsOctree.AddItem(star);
        }
    }
}