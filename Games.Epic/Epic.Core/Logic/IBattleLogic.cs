using System;
using System.Threading;
using System.Threading.Tasks;
using Epic.Core.ClientMessages;
using Epic.Core.Services.Connection;
using JetBrains.Annotations;

namespace Epic.Core.Logic
{
    public interface IBattleLogic : IDisposable
    {
        Task OnClientMessage(IBattleClientConnection connection, IClientBattleMessage clientBattleMessage);
        
        Task<BattleResult> Run(CancellationToken cancellationToken);
    }
}