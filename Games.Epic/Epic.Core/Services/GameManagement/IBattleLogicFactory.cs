using Epic.Core.Logic;
using Epic.Core.Services.Battles;

namespace Epic.Core.Services.GameManagement
{
    public interface IBattleLogicFactory
    {
        IBattleLogic Create(MutableBattleObject battleObject, IBattleMessageBroadcaster broadcaster);
    }
}