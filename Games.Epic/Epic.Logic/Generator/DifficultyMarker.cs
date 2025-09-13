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
            var maxDifficulty = 24 * difficultyFactor * MathExtended.Sqr(day) + 1000;
            
            var range = maxDifficulty - minDifficulty;
            
            var normalizedMean = 1.0 / 4.0; // Bias toward lower part
            var normalizedWideMean = 1.0 / 2.0;
            var narrowStd = 0.3; // how chaotic the output
            var wideStd = 0.7;
            var wideStdChance = 0.1;
            
            var isWideStd = random.NextDouble() < wideStdChance;
            
            var mean = isWideStd ? normalizedWideMean : normalizedMean;
            var std = isWideStd ? wideStd : narrowStd;
            var min = isWideStd ? normalizedMean : 0;
            var sample = RandomDistributions.GetBoundedNormal(random, mean, std , min, 1);

            var idealSample = (int)(minDifficulty + normalizedMean * (maxDifficulty - minDifficulty));
            var targetDifficulty = (int)(minDifficulty + sample * range);

            return new DifficultyMarker
            {
                TargetDifficulty = targetDifficulty,
                IdealDifficulty = idealSample,
                MinDifficulty = (int)minDifficulty,
                MaxDifficulty = (int)maxDifficulty,
            };
        }
    }
}