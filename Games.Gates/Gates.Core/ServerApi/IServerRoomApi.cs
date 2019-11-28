using System.Threading;
using System.Threading.Tasks;

namespace Gates.Core.ServerApi
{
    public interface IServerRoomApi
    {
        Task LeaveRoomAsync(CancellationToken cancellationToken);

        Task<RoomState> GetStateAsync(CancellationToken cancellationToken);

        Task RunGatesGameAsync(CancellationToken cancellationToken);

        Task<IServerGatesApi> ConnectToRunnedGameAsync(CancellationToken cancellationToken);
    }
}
