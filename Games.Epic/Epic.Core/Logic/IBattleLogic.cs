using System;
using System.Threading;
using System.Threading.Tasks;
using Epic.Core.ClientMessages;
using Epic.Core.Objects.BattleClientConnection;
using Epic.Core.ServerMessages;

namespace Epic.Core.Logic
{
    public interface IBattleLogic
    {
        event Action<IServerBattleMessage> BroadcastMessage;
        Task OnClientMessage(IBattleClientConnection connection, IClientBattleMessage clientBattleMessage);
        
        Task Run(CancellationToken cancellationToken);
    }
}