using System;
using Gates.Core.ServerApi;

namespace Gates.ClientCore.Rooms
{
    internal interface IRoomController : IDisposable
    {
        bool IsGameConnected { get; }

        IServerGatesApi GetActiveGameApi();

        void StartListenRoom(IServerRoomApi roomApi);
    }
}
