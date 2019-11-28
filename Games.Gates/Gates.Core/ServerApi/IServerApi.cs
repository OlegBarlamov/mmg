using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gates.Core.ServerApi
{
    public interface IServerApi
    {
        Task<IServerRoomApi> CreateRoomAsync(string name, string password, CancellationToken cancellationToken);

        Task<IServerRoomApi> EnterRoomAsync(string name, string password, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<RoomDescription>> GetRoomsAsync(CancellationToken cancellationToken);
    }
}
