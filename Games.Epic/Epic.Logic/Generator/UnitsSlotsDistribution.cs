using System;
using System.Collections.Generic;
using System.Linq;

namespace Epic.Logic.Generator
{
    public static class UnitsSlotsDistribution
    {
        public static int[] FindSlotIndices(int targetSlotsCount, int height)
        {
            // 2. Spread filled slots evenly across the container height
            List<int> slotIndices = new List<int>();
            for (int i = 0; i < targetSlotsCount; i++)
            {
                if (targetSlotsCount == 1)
                {
                    // Just place the single slot in the middle
                    slotIndices.Add(height / 2);
                }
                else
                {
                    int slotIndex = (int)Math.Round(i * (height - 1.0) / (targetSlotsCount - 1));
                    slotIndices.Add(slotIndex);
                }
            }
            
            return slotIndices.Distinct().ToArray();
        }
    }
}