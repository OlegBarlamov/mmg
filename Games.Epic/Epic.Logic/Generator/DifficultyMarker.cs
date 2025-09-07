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
            var difficultyFactor = 1;
            var minDifficulty = 2 * difficultyFactor * MathExtended.Sqr(day) + 100;
            var maxDifficulty = 20 * difficultyFactor * MathExtended.Sqr(day) + 1000;
            
            var range = maxDifficulty - minDifficulty;
            
            var normalizedMean = 1.0 / 4.0; // Bias toward lower part
            var narrowStd = 0.12; // how chaotic the output
            var wideStd = 0.45;
            var wideStdChance = 0.05;
            
            var isWideStd = random.NextDouble() < wideStdChance; 

            var sample = RandomDistributions.GetBoundedNormal(random, normalizedMean, isWideStd ? wideStd : narrowStd, 0, 1);

            var idealSample = (int)(minDifficulty + normalizedMean * (maxDifficulty - minDifficulty));
            var targetDifficulty = (int)(minDifficulty + sample * range);

            return new DifficultyMarker
            {
                TargetDifficulty = targetDifficulty,
                IdealDifficulty = idealSample,
                MinDifficulty = minDifficulty,
                MaxDifficulty = (int)maxDifficulty,
            };
        }
    }
}