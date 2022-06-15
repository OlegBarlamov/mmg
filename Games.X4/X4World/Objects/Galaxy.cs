using System;
using System.Collections.Generic;
using FrameworkSDK;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGameExtensions;
using X4World.Maps;

namespace X4World.Objects
{
    public class Galaxy : ILocatable3D, INamed
    {
        public string Name { get; }
    
        public Vector3 Position { get; }
        
        public Vector3 Size { get; } = new Vector3(10);
        
        public GalaxiesMapCell OwnedCell { get; }

        public IReadOnlyList<Star> Stars { get; }
        private readonly List<Star> _stars = new List<Star>();

        public Galaxy([NotNull] GalaxiesMapCell cell, Vector3 worldPosition, string name)
        {
            OwnedCell = cell ?? throw new ArgumentNullException(nameof(cell));
            Position = worldPosition;
            Name = name;

            Stars = _stars;
        }

        public void AddStar([NotNull] Star star)
        {
            if (star == null) throw new ArgumentNullException(nameof(star));
            _stars.Add(star);
        }
    }
}