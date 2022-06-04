using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Atom.Client.MacOS.Services
{
    public interface IAstronomicalMapUpdater
    {
        MapUpdateResult Update(GameTime gameTime);
    }

    public class MapUpdateResult
    {
        public IReadOnlyList<AstronomicalMapCell> AddedPoints { get; }
        
        public IReadOnlyList<AstronomicalMapCell> RemovedPoints { get; }

        public MapUpdateResult([NotNull] IReadOnlyList<AstronomicalMapCell> addedPoints, [NotNull] IReadOnlyList<AstronomicalMapCell> removedPoints)
        {
            AddedPoints = addedPoints ?? throw new ArgumentNullException(nameof(addedPoints));
            RemovedPoints = removedPoints ?? throw new ArgumentNullException(nameof(removedPoints));
        }
    }
}