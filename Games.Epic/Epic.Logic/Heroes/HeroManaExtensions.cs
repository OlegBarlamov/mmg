using System;
using Epic.Data.Heroes;

namespace Epic.Logic.Heroes
{
    /// <summary>Max mana = MaxManaPerKnowledge * Knowledge. Hero restores ManaRestoredPerKnowledge * Knowledge mana (e.g. at round start).</summary>
    public static class HeroManaConstants
    {
        public const int MaxManaPerKnowledge = 10;
        public const int ManaRestoredPerKnowledge = 1;
    }

    public static class HeroManaExtensions
    {
        /// <summary>Computes max mana from knowledge: MaxManaPerKnowledge * Knowledge.</summary>
        public static int GetMaxMana(this IHeroStats stats)
            => HeroManaConstants.MaxManaPerKnowledge * stats.Knowledge;

        /// <summary>Returns current mana after restoring: min(currentMana + knowledge, maxMana). One mana per knowledge.</summary>
        public static int GetCurrentManaAfterRestore(int currentMana, int knowledge, int maxMana)
            => Math.Min(currentMana + knowledge, maxMana);

        /// <summary>Returns current mana after restoring using stats.</summary>
        public static int GetCurrentManaAfterRestore(this IHeroStats stats)
            => GetCurrentManaAfterRestore(stats.CurrentMana, stats.Knowledge, stats.GetMaxMana());
    }
}
