using Epic.Data.Heroes;

namespace Epic.Core.Objects
{
    public class DefaultHeroStats : IHeroStats
    {
        public static DefaultHeroStats Instance { get; } = new DefaultHeroStats();
        
        public int Attack => 0;
        public int Defense => 0;
        public int Level => 1;
        public int Experience => 0;
        public int Power => 0;
        public int Knowledge => 0;
        public int CurrentMana => 0;
    }
}