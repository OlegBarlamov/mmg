using System;
using Epic.Logic.Utils;
using NetExtensions.Helpers;

namespace Epic.Logic.Generator
{
    public class DifficultyMarker
    {
        public int MinDifficulty { get; private set; }
        public int MaxDifficulty { get; private set; }
        public int TargetDifficulty { get; private set; }
        public int IdealDifficulty { get; private set; }

        public static DifficultyMarker GenerateFromDay(Random random, int day)
        {
            var minDifficulty = 3 * MathExtended.Sqr(day) + 100;
            var maxDifficulty = 30 * MathExtended.Sqr(day) + 1000;
            
            var range = maxDifficulty - minDifficulty;

            var normalizedMean = 1.0 / 4.0; // Bias toward lower part
            var stdDev = 0.15; // how chaotic the output

            var sample = RandomDistributions.GetBoundedNormal(random, normalizedMean, stdDev, 0, 1);

            var idealSample = (int)(minDifficulty + normalizedMean * (maxDifficulty - minDifficulty));
            var targetDifficulty = (int)(minDifficulty + sample * range);

            return new DifficultyMarker
            {
                TargetDifficulty = targetDifficulty,
                IdealDifficulty = idealSample,
                MinDifficulty = minDifficulty,
                MaxDifficulty = maxDifficulty,
            };
        }
    }
}