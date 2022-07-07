using System;

namespace X4World.Objects
{
    public class WorldMapCellAggregatedData : IDisposable
    {
        public WorldMapCellTextureData WorldMapCellTextureData { get; } = new WorldMapCellTextureData();

        public byte[,,] SubstanceMap { get; }

        public WorldMapCellAggregatedData(byte[,,] data)
        {
            SubstanceMap = data;
        }

        public void Dispose()
        {
            WorldMapCellTextureData.Dispose();
        }
    }
}