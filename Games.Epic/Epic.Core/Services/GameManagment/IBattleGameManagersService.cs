using System.Threading.Tasks;
using Epic.Core.Objects.BattleClientConnection;
using Epic.Core.Objects.BattleGameManager;

namespace Epic.Core
{
    public interface IBattleGameManagersService
    {
        Task<IBattleGameManager> GetBattleGameManager(IBattleClientConnection clientConnection);
        Task RemoveClientConnection(IBattleClientConnection clientConnection);
    }
}