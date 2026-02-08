using Epic.Data.Heroes;
using Epic.Logic.Heroes;

namespace Epic.Server.Resources
{
    /// <summary>API DTO for hero stats. MaxMana is computed via Logic extension (not stored).</summary>
    public class HeroStatsResource
    {
        public int Attack { get; }
        public int Defense { get; }
        public int Level { get; }
        public int Experience { get; }
        public int Power { get; }
        public int Knowledge { get; }
        public int CurrentMana { get; }
        public int MaxMana { get; }

        public HeroStatsResource(IHeroStats stats)
        {
            Attack = stats.Attack;
            Defense = stats.Defense;
            Level = stats.Level;
            Experience = stats.Experience;
            Power = stats.Power;
            Knowledge = stats.Knowledge;
            CurrentMana = stats.CurrentMana;
            MaxMana = stats.GetMaxMana();
        }
    }
}
