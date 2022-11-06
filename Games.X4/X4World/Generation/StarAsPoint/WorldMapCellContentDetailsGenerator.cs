using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NetExtensions.Collections;
using X4World.Objects;

namespace X4World.Generation
{
    public class WorldMapCellContentDetailsGenerator : IDetailsGenerator<WorldMapCellContent>
    {
        public void Generate(WorldMapCellContent target)
        {
            var aggregatedData = target.WorldMapCellAggregatedData;
            var objects = new List<StarAsPoint>();

            foreach (var dataGalaxiesPoint in aggregatedData.GalaxiesPoints)
            {
                var itemAggregatedData = new StarAsPointAggregatedData(1);
                var item = new StarAsPoint(target, dataGalaxiesPoint.LocalPositionFromCenter, itemAggregatedData);
                objects.Add(item);
            }

            target.SetGeneratedData(objects);
        }

        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((WorldMapCellContent) target);
        }
    }
}