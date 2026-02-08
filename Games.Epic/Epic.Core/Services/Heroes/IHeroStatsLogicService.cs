using Epic.Data.Heroes;

namespace Epic.Core.Services.Heroes
{
    /// <summary>Logic for hero stats (e.g. mana restoration). Implementation lives in Epic.Logic.</summary>
    public interface IHeroStatsLogicService
    {
        /// <summary>Returns current mana after restoring: min(currentMana + knowledge, maxMana). One mana per knowledge.</summary>
        int GetCurrentManaAfterRestore(IHeroStats stats);
    }
}
