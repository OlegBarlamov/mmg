using System;

namespace Gates.ClientCore.ExternalCommands
{
    public interface IClientHost : IDisposable
    {
        void ConnectToServerAsync(string serverUrl);

        void AuthorizeAsync(string name);

        void CreateRoomAsync(string name, string password);

        void EnterRoomAsync(string name, string password);

        void RunGameAsync();
    }
}
