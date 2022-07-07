using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace X4World.Objects
{
    public class WorldMapCellAggregatedData : IDisposable
    {
        public class GalaxyPointData
        {
            public Vector3 LocalPositionFromCenter { get; }
            public float Temperature { get; }

            public GalaxyPointData(Vector3 localPositionFromCenter, float temperature)
            {
                LocalPositionFromCenter = localPositionFromCenter;
                Temperature = temperature;
            }
        } 
        
        public WorldMapCellTextureData WorldMapCellTextureData { get; } = new WorldMapCellTextureData();

        public IReadOnlyCollection<GalaxyPointData> GalaxiesPoints { get; }

        public WorldMapCellAggregatedData([NotNull] IReadOnlyCollection<GalaxyPointData> galaxiesPoints)
        {
            GalaxiesPoints = galaxiesPoints ?? throw new ArgumentNullException(nameof(galaxiesPoints));
        }

        public void Dispose()
        {
            WorldMapCellTextureData.Dispose();
        }
    }
}