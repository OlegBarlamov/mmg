using Epic.Core.Logic;
using Epic.Core.Objects.Battle;

namespace Epic.Core.Services.GameManagement
{
    public interface IBattleLogicFactory
    {
        IBattleLogic Create(MutableBattleObject battleObject, IBattleMessageBroadcaster broadcaster);
    }
}