using Epic.Core.Logic;
using Epic.Core.Objects.Battle;
using Epic.Core.Objects.BattleGameManager;

namespace Epic.Core.Services
{
    public interface IBattleLogicFactory
    {
        IBattleLogic Create(MutableBattleObject battleObject, IBattleMessageBroadcaster broadcaster);
    }
}