using System;
using System.Threading.Tasks;
using Epic.Core.Objects.Battle;
using Epic.Core.Objects.BattleClientConnection;

namespace Epic.Core.Objects.BattleGameManager
{
    public interface IBattleGameManager : IDisposable
    {
        event Action<IBattleGameManager> Finished;
        Guid BattleId { get; }
        int GetClientsCount(); 
        bool IsBattlePlaying();
        void PlayBattle();
        void SuspendBattle();
    
        Task AddClient(IBattleClientConnection connection);
        Task RemoveClient(IBattleClientConnection connection);
    }
}