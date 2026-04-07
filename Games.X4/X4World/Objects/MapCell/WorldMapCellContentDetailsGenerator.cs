using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using X4World.Maps;

namespace X4World.Objects
{
    public class WorldMapCellContentDetailsGenerator : IDetailsGenerator<WorldMapCellContent>
    {
        public void Generate(WorldMapCellContent target)
        {
            var aggregatedData = target.WorldMapCellAggregatedData;
            var galaxyPoints = aggregatedData.GalaxiesPoints.ToArray();
            var cellSize = WorldConstants.WorldMapCellSize;

            var batchData = new GalaxiesBatchAggregatedData(cellSize, galaxyPoints);
            var batch = new GalaxiesBatch(target, Vector3.Zero, cellSize * 1.5f, batchData);

            target.SetGeneratedData(new List<IWrappedDetails> { batch });
        }

        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((WorldMapCellContent) target);
        }
    }
}
