using System;
using System.Threading;
using System.Threading.Tasks;
using Epic.Core.ClientMessages;
using Epic.Core.Objects.BattleClientConnection;
using Epic.Core.ServerMessages;

namespace Epic.Core.Logic
{
    public interface IBattleLogic : IDisposable
    {
        event Action<IServerBattleMessage> BroadcastMessage;
        Task OnClientMessage(IBattleClientConnection connection, IClientBattleMessage clientBattleMessage);
        
        Task<BattleResult> Run(CancellationToken cancellationToken);
    }
}