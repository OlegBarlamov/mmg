using System;
using Gates.ClientCore.Rooms;
using Gates.ClientCore.ServerConnection;
using Gates.Core.ServerApi;
using JetBrains.Annotations;

namespace Gates.ClientCore.ExternalCommands
{
    [UsedImplicitly]
    internal class ClientHost : IClientHost
    {
        private  IServerConnector ServerConnector { get; }
        private IRoomController RoomController { get; }

        private IServer _baseServer;
        private IServerApi _serverApi;
        private IServerRoomApi _serverRoomApi;

        public ClientHost([NotNull] IServerConnector serverConnector, [NotNull] IRoomController roomController)
        {
            ServerConnector = serverConnector ?? throw new ArgumentNullException(nameof(serverConnector));
            RoomController = roomController ?? throw new ArgumentNullException(nameof(roomController));
        }

        public void Dispose()
        {

        }

        public void ConnectToServer(string serverUrl)
        {
            lock (_baseServer)
            {
                if (_baseServer != null)
                    throw new ClientHostException("You are already connected to the server. Disconnect first.");

                _baseServer = ServerConnector.Connect(serverUrl);
            }
        }

        public void Authorize(string name)
        {
            if (_baseServer == null)
                throw new ClientHostException("Connect to the server first.");

            lock (_serverApi)
            {
                if (_serverApi != null)
                    throw new ClientHostException("You are already authorized on the server. Log out first.");

                _serverApi = _baseServer.Authorize(name, string.Empty);
            }
        }

        public void CreateRoom(string name, string password)
        {
            if (_serverApi == null)
                throw new ClientHostException("Log in first.");

            lock (_serverRoomApi)
            {
                if (_serverRoomApi != null)
                    throw new ClientHostException("Leave the room before creating a new one.");

                _serverRoomApi = _serverApi.CreateRoom(name, password);
                if (_serverRoomApi != null)
                    RoomController.StartListenRoom(_serverRoomApi);
            }
        }

        public void EnterRoom(string name, string password)
        {
            if (_serverApi == null)
                throw new ClientHostException("Log in first.");

            lock (_serverRoomApi)
            {
                if (_serverRoomApi != null)
                    throw new ClientHostException("Leave the room before entering another.");

                _serverRoomApi = _serverApi.EnterRoom(name, password);
                if (_serverRoomApi != null)
                    RoomController.StartListenRoom(_serverRoomApi);
            }
        }

        public void RunGame()
        {
            if (_serverRoomApi == null)
                throw new ClientHostException("You are not in any of the rooms.");

            lock (_serverRoomApi)
            {
                _serverRoomApi.RunGatesGame();
            }
        }
    }
}
