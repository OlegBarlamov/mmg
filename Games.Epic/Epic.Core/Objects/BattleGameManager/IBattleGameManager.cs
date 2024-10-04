using System;
using System.Threading.Tasks;
using Epic.Core.Objects.Battle;
using Epic.Core.Objects.BattleClientConnection;

namespace Epic.Core.Objects.BattleGameManager
{
    public interface IBattleGameManager : IDisposable
    {
        bool IsDisposed { get; }
        IBattleObject BattleObject { get; }
        int GetClientsCount(); 
        Task PlayBattle();
        Task SuspendBattle();
    
        Task AddClient(IBattleClientConnection connection);
        Task RemoveClient(IBattleClientConnection connection);
    }
}