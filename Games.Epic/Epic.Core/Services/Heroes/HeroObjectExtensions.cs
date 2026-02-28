using System;
using System.Linq;
using Epic.Core.Objects;
using Epic.Core.Services.Artifacts;
using Epic.Data.Heroes;

namespace Epic.Core.Services.Heroes
{
    public static class HeroObjectExtensions
    {
        public static Guid[] GetEffectiveKnownMagicTypeIds(this IHeroObject hero)
        {
            if (hero == null) return Array.Empty<Guid>();
            var learned = hero.KnownMagicTypeIds ?? Array.Empty<Guid>();
            var fromArtifacts = (hero.GetEquippedArtefacts() ?? Array.Empty<IArtifactObject>())
                .SelectMany(a => a?.ArtifactType?.GrantMagicTypeIds ?? Array.Empty<Guid>());
            return learned.Union(fromArtifacts).Distinct().ToArray();
        }

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

        public static int GetEquippedArtifactsKnowledgeBonus(this IHeroObject hero)
        {
            if (hero == null) return 0;
            return hero.GetEquippedArtefacts()
                .Sum(a => a?.ArtifactType?.KnowledgeBonus ?? 0);
        }

        public static int GetEquippedArtifactsPowerBonus(this IHeroObject hero)
        {
            if (hero == null) return 0;
            return hero.GetEquippedArtefacts()
                .Sum(a => a?.ArtifactType?.PowerBonus ?? 0);
        }

        public static int GetEquippedArtifactsManaRestorationBonus(this IHeroObject hero)
        {
            if (hero == null) return 0;
            return hero.GetEquippedArtefacts()
                .Sum(a => a?.ArtifactType?.ManaRestorationBonus ?? 0);
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
            return new HeroStatsSnapshot(
                hero.GetCumulativeAttack(),
                hero.GetCumulativeDefense(),
                hero.Level,
                hero.Experience,
                hero.Power + hero.GetEquippedArtifactsPowerBonus(),
                hero.Knowledge + hero.GetEquippedArtifactsKnowledgeBonus(),
                hero.CurrentMana,
                hero.GetEquippedArtifactsManaRestorationBonus());
        }

        private sealed class HeroStatsSnapshot : IHeroStats
        {
            public int Attack { get; }
            public int Defense { get; }
            public int Level { get; }
            public int Experience { get; }
            public int Power { get; }
            public int Knowledge { get; }
            public int CurrentMana { get; }
            public int ManaRestorationBonus { get; }

            public HeroStatsSnapshot(int attack, int defense, int level, int experience, int power, int knowledge, int currentMana, int manaRestorationBonus = 0)
            {
                Attack = attack;
                Defense = defense;
                Level = level;
                Experience = experience;
                Power = power;
                Knowledge = knowledge;
                CurrentMana = currentMana;
                ManaRestorationBonus = manaRestorationBonus;
            }
        }
    }
}

