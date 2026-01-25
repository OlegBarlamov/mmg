using System;
using System.Linq;
using Epic.Core.Objects;
using Epic.Data.Heroes;

namespace Epic.Core.Services.Heroes
{
    public static class HeroObjectExtensions
    {
        public static int GetEquippedArtifactsAttackBonus(this IHeroObject hero)
        {
            if (hero == null) return 0;
            return hero.GetEquippedArtefacts()
                .Sum(a => a?.ArtifactType?.AttackBonus ?? 0);
        }

        public static int GetEquippedArtifactsDefenseBonus(this IHeroObject hero)
        {
            if (hero == null) return 0;
            return hero.GetEquippedArtefacts()
                .Sum(a => a?.ArtifactType?.DefenseBonus ?? 0);
        }

        public static int GetCumulativeAttack(this IHeroObject hero)
        {
            if (hero == null) return 0;
            return hero.Attack + hero.GetEquippedArtifactsAttackBonus();
        }

        public static int GetCumulativeDefense(this IHeroObject hero)
        {
            if (hero == null) return 0;
            return hero.Defense + hero.GetEquippedArtifactsDefenseBonus();
        }

        public static IHeroStats GetCumulativeHeroStats(this IHeroObject hero)
        {
            if (hero == null) return DefaultHeroStats.Instance;
            return new HeroStatsSnapshot(hero.GetCumulativeAttack(), hero.GetCumulativeDefense());
        }

        private sealed class HeroStatsSnapshot : IHeroStats
        {
            public int Attack { get; }
            public int Defense { get; }

            public HeroStatsSnapshot(int attack, int defense)
            {
                Attack = attack;
                Defense = defense;
            }
        }
    }
}

