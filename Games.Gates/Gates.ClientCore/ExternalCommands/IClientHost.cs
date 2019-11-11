using System;

namespace Gates.ClientCore.ExternalCommands
{
    public interface IClientHost : IDisposable
    {
        void ConnectToServer(string serverUrl);

        void Authorize(string name);

        void CreateRoom(string name, string password);

        void EnterRoom(string name, string password);

        void RunGame();
    }
}
