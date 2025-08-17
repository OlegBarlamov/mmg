using System;
using Epic.Core.Logic;
using Epic.Core.Services.Heroes;
using Epic.Logic.Generator;
using FrameworkSDK.Common;
using JetBrains.Annotations;

namespace Epic.Logic.Heroes
{
    [UsedImplicitly]
    public class HeroesLevelsCalculator : IHeroesLevelsCalculator
    {
        private IRandomService RandomProvider { get; }

        public HeroesLevelsCalculator([NotNull] IRandomService randomProvider)
        {
            RandomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider));
        }
        
        public IExperienceGainResult GiveExperience(IHeroObject hero, int experienceGain)
        {
            int gainedExp = experienceGain;
            int gainedLevels = 0, gainedAttacks = 0, gainedDefense = 0;

            var currentExp = hero.Experience + gainedExp;
            int currentLevel = hero.Level;
            
            int requiredXp = hero.Level == 1
                ? BaseXpForLevel2
                : GetXpRequiredForNextLevel(currentLevel, GetTotalXpForLevel(currentLevel));

            while (currentExp >= requiredXp)
            {
                currentExp -= requiredXp;
                gainedLevels++;
                currentLevel++;

                // Randomly choose Attack or Defense
                if (RandomProvider.NextInteger(0, 2) == 0)
                {
                    gainedAttacks++;
                }
                else
                {
                    gainedDefense++;
                }

                requiredXp = GetXpRequiredForNextLevel(currentLevel, GetTotalXpForLevel(currentLevel));
            }

            return new ExperienceGainResult(gainedExp, gainedLevels, gainedAttacks, gainedDefense);
        }
        
        private const int BaseXpForLevel2 = 100;
        private int GetXpRequiredForNextLevel(int level, int prevTotalXp)
        {
            double factor;
            switch (level)
            {
                case 1: factor = 1.00;    break;
                case 2: factor = 0.95;    break; 
                case 3: factor = 0.90;    break;
                case 4: factor = 0.85;    break;
                case 5: factor = 0.80;    break;
                case 6: factor = 0.75;    break;
                case 7: factor = 0.70;    break;
                case 8: factor = 0.65;    break;
                case 9: factor = 0.60;    break;
                case 10: factor = 0.55;    break;
                case 11: factor = 0.50;    break;
                case 12: factor = 0.45;    break;
                case 13: factor = 0.40;    break;
                case 14: factor = 0.35;    break;
                case 15: factor = 0.30;    break;
                case 16: factor = 0.25;    break;
                default: factor = 0.20;    break;
            }
            return (int)(prevTotalXp * (1 + factor));
        }
        
        private int GetTotalXpForLevel(int level)
        {
            if (level <= 1) return 0;
            int totalXp = BaseXpForLevel2;
            for (int i = 2; i < level; i++)
            {
                totalXp = GetXpRequiredForNextLevel(i, totalXp);
            }
            return totalXp;
        }
    }

    internal class ExperienceGainResult : IExperienceGainResult
    {
        public int ExperienceGain { get; }
        public int LevelsGain { get; }
        public int AttacksGain { get; }
        public int DefenseGain { get; }

        public ExperienceGainResult(int exp, int levels, int attacks, int defense)
        {
            ExperienceGain = exp;
            LevelsGain = levels;
            AttacksGain = attacks;
            DefenseGain = defense;
        }
    }
}