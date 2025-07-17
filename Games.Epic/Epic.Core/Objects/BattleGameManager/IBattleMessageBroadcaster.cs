using System.Threading.Tasks;
using Epic.Core.ServerMessages;

namespace Epic.Core.Objects.BattleGameManager
{
    public interface IBattleMessageBroadcaster
    {
        Task BroadcastMessageAsync(IServerBattleMessage message);
    }
}