using System;

namespace Epic.Logic.Utils
{
    internal static class RandomDistributions
    {
        public static double GetBoundedNormal(Random random, double mean, double stdDev, double min, double max)
        {
            double value;
            do
            {
                double u1 = 1.0 - random.NextDouble();
                double u2 = 1.0 - random.NextDouble();
                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                       Math.Sin(2.0 * Math.PI * u2);
                value = mean + stdDev * randStdNormal;
            }
            while (value < min || value > max);

            return value;
        }
    }
}