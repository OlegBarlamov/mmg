using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using X4World;
using X4World.Maps;
using X4World.Objects;

namespace Atom.Client.MacOS.Services
{
    public interface IMapUpdater
    {
        MapUpdateResult Update(GameTime gameTime);
    }

    public class MapUpdateResult
    {
        public IReadOnlyList<GalaxiesMapCell> AddedPoints { get; }
        
        public IReadOnlyList<GalaxiesMapCell> RemovedPoints { get; }
        
        public IReadOnlyList<Star> StarsRemoved { get; }
        
        public IReadOnlyList<Star> StarsAdded { get; }

        public MapUpdateResult([NotNull] IReadOnlyList<GalaxiesMapCell> addedPoints, [NotNull] IReadOnlyList<GalaxiesMapCell> removedPoints,
            [NotNull] IReadOnlyList<Star> starsAdded, [NotNull] IReadOnlyList<Star> starsRemoved)
        {
            AddedPoints = addedPoints ?? throw new ArgumentNullException(nameof(addedPoints));
            RemovedPoints = removedPoints ?? throw new ArgumentNullException(nameof(removedPoints));
            StarsRemoved = starsRemoved ?? throw new ArgumentNullException(nameof(starsRemoved));
            StarsAdded = starsAdded ?? throw new ArgumentNullException(nameof(starsAdded));
        }
    }
}