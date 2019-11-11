using System;
using Gates.Core.ServerApi;

namespace Gates.ClientCore.Rooms
{
    internal interface IRoomController : IDisposable
    {
        void StartListenRoom(IServerRoomApi roomApi);
    }
}
