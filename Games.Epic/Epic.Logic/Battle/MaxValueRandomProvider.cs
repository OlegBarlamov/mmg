using System;
using FrameworkSDK.Common;

namespace Epic.Logic.Battle
{
    internal class MaxValueRandomProvider : IRandomService
    {
        public int NextInteger(int minValue, int maxValue)
        {
            return maxValue - 1;
        }

        public double NextDouble()
        {
            return 1.0;
        }

        public Guid NewGuid()
        {
            return Guid.Empty;
        }

        public float NextFloat(float minValue, float maxValue)
        {
            return maxValue;
        }
    }
}