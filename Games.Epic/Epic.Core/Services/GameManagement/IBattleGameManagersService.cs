using System.Threading.Tasks;
using Epic.Core.Services.Connection;

namespace Epic.Core.Services.GameManagement
{
    public interface IBattleGameManagersService
    {
        Task<IBattleGameManager> GetOrCreateBattleGameManager(IBattleClientConnection clientConnection);
        Task RemoveClientConnection(IBattleClientConnection clientConnection);
    }
}