using Epic.Core.Services.Heroes;
using Epic.Data.Heroes;
using JetBrains.Annotations;

namespace Epic.Logic.Heroes
{
    [UsedImplicitly]
    public class HeroStatsLogicService : IHeroStatsLogicService
    {
        public int GetCurrentManaAfterRestore(IHeroStats stats)
            => stats.GetCurrentManaAfterRestore();
    }
}
