using System;
using System.Collections.Generic;
using System.Linq;

namespace HexoGrid
{
    public static class HexGridTypeExtensions
    {
        private static readonly Dictionary<HexGridType, HexPoint[][]> OffsetsAggByParity;

        static HexGridTypeExtensions()
        {
            OffsetsAggByParity = CalculateOffsetsAggByParity();
        }

        public static HexPoint[][] GetOffsetsAggByParity(this HexGridType gridType)
        {
            return OffsetsAggByParity[gridType];
        }

        private static Dictionary<HexGridType, HexPoint[][]> CalculateOffsetsAggByParity()
        {
            var result = new Dictionary<HexGridType, HexPoint[][]>();
            var gridTypeValues = Enum.GetValues(typeof(HexGridType)).Cast<HexGridType>();
            foreach (var gridType in gridTypeValues)
            {
                var value = CalculateOffsetsAggByParity(gridType);
                result.Add(gridType, value);
            }
            return result;
        }

        private static HexPoint[][] CalculateOffsetsAggByParity(HexGridType gridType)
        {
            switch (gridType)
            {
                case HexGridType.HorizontalOdd:
                    return new[]
                    {
                        HexOffsets.HorizontalOdd.EvenRow.GetAll(),
                        HexOffsets.HorizontalOdd.OddRow.GetAll()
                    };
                case HexGridType.HorizontalEven:
                    return new[]
                    {
                        HexOffsets.HorizontalEven.EvenRow.GetAll(),
                        HexOffsets.HorizontalEven.OddRow.GetAll()
                    };
                case HexGridType.VerticalOdd:
                    return new[]
                    {
                        HexOffsets.VerticalOdd.EvenCol.GetAll(),
                        HexOffsets.VerticalOdd.OddCol.GetAll()
                    };
                case HexGridType.VerticalEven:
                    return new[]
                    {
                        HexOffsets.VerticalEven.EvenCol.GetAll(),
                        HexOffsets.VerticalEven.OddCol.GetAll()
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(gridType), gridType, null);
            }
        }
    }
}
