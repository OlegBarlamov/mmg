using System;
using Epic.Core.Logic;
using Epic.Logic.Utils;
using NetExtensions.Helpers;

namespace Epic.Logic.Generator
{
    public class DifficultyMarker
    {
        public int MinDifficulty { get; set; }
        public int MaxDifficulty { get; set; }
        public int TargetDifficulty { get; set; }
        public int IdealDifficulty { get; set; }

        public static DifficultyMarker GenerateFromDay(Random random, IGameModeStage stage, int day)
        {
            var difficultyFactor = stage.DifficultyFactor;
            
            var minDifficulty = stage.MinDifficultyFactor * difficultyFactor * MathExtended.Sqr(day) + stage.StartMinDifficulty;
            var maxDifficulty = stage.MaxDifficultyFactor * difficultyFactor * MathExtended.Sqr(day) + stage.StartMaxDifficulty;
            
            var range = maxDifficulty - minDifficulty;
            
            var normalizedMean = 1.0 / 4.0; // Bias toward lower part
            var normalizedWideMean = 1.25 / 2.0;
            var narrowStd = 0.2; // how chaotic the output
            var wideStd = 0.6;
            var wideStdChance = (double)stage.WideStdChance / 100;
            
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

        public static DifficultyMarker FromFixedDifficulty(int fixedDifficulty)
        {
            return new DifficultyMarker
            {
                TargetDifficulty = fixedDifficulty,
                MinDifficulty = fixedDifficulty,
                IdealDifficulty = fixedDifficulty,
                MaxDifficulty = fixedDifficulty
            };
        }
    }
}