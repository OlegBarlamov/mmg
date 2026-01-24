using Epic.Core.Services.Battles;

namespace Epic.Core.Logic
{
    public interface IBattleUnitsPlacer
    {
        void PlaceBattleUnitsDefaultPattern(
            MutableBattleObject battleObject,
            int? player1OriginalContainerHeight = null,
            int? player2OriginalContainerHeight = null);
    }
}