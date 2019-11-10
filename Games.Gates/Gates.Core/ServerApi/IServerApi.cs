using System.Collections.Generic;

namespace Gates.Core.ServerApi
{
    public interface IServerApi
    {
        IServerRoomApi CreateRoom(string name, string password);

        IServerRoomApi EnterRoom(string name, string password);

        IReadOnlyCollection<RoomDescription> Rooms();
    }
}
