using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NetExtensions.Collections;
using X4World.Objects;

namespace X4World.Generation
{
    public class WorldMapCellContentDetailsGenerator : IDetailsGenerator<WorldMapCellContent>
    {
        private IDetailsGenerator<WorldMapCellContent> _detailsGeneratorImplementation;

        public void Generate(WorldMapCellContent target)
        {
            var aggregatedData = target.WorldMapCellAggregatedData;
            var objects = new List<GalaxyAsPoint>();
            
            aggregatedData.SubstanceMap.For((data, x, y, z) =>
            {
                if (data == 0)
                    return false;
                
                var deltaX = target.Size.X / aggregatedData.SubstanceMap.GetLength(0);
                var deltaY = target.Size.Y / aggregatedData.SubstanceMap.GetLength(1);
                var deltaZ = target.Size.Z / aggregatedData.SubstanceMap.GetLength(2);
                var localPosition = new Vector3(x * deltaX - target.Size.X / 2, y * deltaY - target.Size.Y / 2, z * deltaZ - target.Size.Z / 2);
                
                var itemAggregatedData = new GalaxyAsPointAggregatedData(data);
                var item = new GalaxyAsPoint(target, localPosition, itemAggregatedData);
                objects.Add(item);
                
                return false;
            });
            
            target.SetGeneratedData(objects);
        }

        void IDetailsGenerator.Generate(IGeneratorTarget target)
        {
            Generate((WorldMapCellContent) target);
        }
    }
}