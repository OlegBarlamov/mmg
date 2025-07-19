using System.Threading.Tasks;
using Epic.Core.ServerMessages;

namespace Epic.Core.Services.GameManagement
{
    public interface IBattleMessageBroadcaster
    {
        Task BroadcastMessageAsync(IServerBattleMessage message);
    }
}