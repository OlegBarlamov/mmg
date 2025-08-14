using Epic.Data.Heroes;

namespace Epic.Core.Objects
{
    public class DefaultHeroStats : IHeroStats
    {
        public static DefaultHeroStats Instance { get; } = new DefaultHeroStats();
        
        public int Attack => 0;
        public int Defense => 0;
    }
}