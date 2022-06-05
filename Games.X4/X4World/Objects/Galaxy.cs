using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using X4World.Maps;

namespace X4World.Objects
{
    public class Galaxy
    {
        public Vector3 WorldPosition { get; }
        
        public Vector3 Size { get; } = new Vector3(5);
        
        public GalaxiesMapCell OwnedCell { get; }

        public IReadOnlyList<Star> Stars { get; }
        private readonly List<Star> _stars = new List<Star>();

        public Galaxy([NotNull] GalaxiesMapCell cell, Vector3 worldPosition)
        {
            OwnedCell = cell ?? throw new ArgumentNullException(nameof(cell));
            WorldPosition = worldPosition;

            Stars = _stars;
        }

        public void AddStar([NotNull] Star star)
        {
            if (star == null) throw new ArgumentNullException(nameof(star));
            _stars.Add(star);
        }
    }
}