using System;
using System.Threading;
using System.Threading.Tasks;
using Gates.ClientCore.Logging;
using Gates.ClientCore.Rooms;
using Gates.Core.ServerApi;
using JetBrains.Annotations;
using Logging;
using Microsoft.Extensions.Logging;
using NetExtensions;

namespace Gates.ClientCore.ExternalCommands
{
    [UsedImplicitly]
    internal class ClientHost : IClientHost
    {
        private IServerConnector ServerConnector { get; }
        private IRoomController RoomController { get; }
        private ILogger Logger { get; }

        private readonly object _baseServerLocker = new object();
        private Task _baseServerTask;
        private IServer _baseServer;
        private readonly object _serverApiLocker = new object();
        private Task _serverApiTask;
        private IServerApi _serverApi;
        private readonly object _serverRoomApiLocker = new object();
        private Task _serverRoomApiTask;
        private IServerRoomApi _serverRoomApi;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public ClientHost([NotNull] IServerConnector serverConnector, [NotNull] IRoomController roomController,
            [NotNull] ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            ServerConnector = serverConnector ?? throw new ArgumentNullException(nameof(serverConnector));
            RoomController = roomController ?? throw new ArgumentNullException(nameof(roomController));

            Logger = loggerFactory.CreateLogger(LogCategories.ExternalCommands);
        }

        public void Dispose()
        {
            using (_cts)
            {
                _cts.Cancel();
            }
        }

        public void ConnectToServerAsync(string serverUrl)
        {
            lock (_baseServerLocker)
            {
                if (_baseServer != null)
                    throw new ClientHostException("You are already connected to the server. Disconnect first.");

                if (_baseServerTask != null && !_baseServerTask.IsFinished())
                    throw new ClientHostException("You are already connecting to the server now...");

                _baseServerTask = StartThread(() =>
                {
                    _baseServer = ServerConnector.ConnectAsync(serverUrl, _cts.Token).Result;
                });
            }
        }

        public void AuthorizeAsync(string name)
        {
            if (_baseServer == null)
                throw new ClientHostException("Connect to the server first.");

            lock (_serverApiLocker)
            {
                if (_serverApi != null)
                    throw new ClientHostException("You are already authorized on the server. Log out first.");

                if (_serverApiTask != null && !_serverApiTask.IsFinished())
                    throw new ClientHostException("You are already authorizing on the server...");

                _serverApiTask = StartThread(() =>
                {
                    _serverApi = _baseServer.AuthorizeAsync(name, string.Empty, _cts.Token).Result;
                });
            }
        }

        public void CreateRoomAsync(string name, string password)
        {
            if (_serverApi == null)
                throw new ClientHostException("Log in first.");

            lock (_serverRoomApiLocker)
            {
                if (_serverRoomApi != null)
                    throw new ClientHostException("Leave the room before creating a new one.");

                if (_serverRoomApiTask != null && !_serverRoomApiTask.IsFinished())
                    throw new ClientHostException("You are already entering or creating the room...");

                _serverRoomApiTask = StartThread(() =>
                {
                    _serverRoomApi = _serverApi.CreateRoomAsync(name, string.Empty, _cts.Token).Result;
                    if (_serverRoomApi != null)
                        RoomController.StartListenRoom(_serverRoomApi);
                });
            }
        }

        public void EnterRoomAsync(string name, string password)
        {
            if (_serverApi == null)
                throw new ClientHostException("Log in first.");

            lock (_serverRoomApiLocker)
            {
                if (_serverRoomApi != null)
                    throw new ClientHostException("Leave the room before entering another.");

                if (_serverRoomApiTask != null && !_serverRoomApiTask.IsFinished())
                    throw new ClientHostException("You are already entering or creating the room...");

                _serverRoomApiTask = StartThread(() =>
                {
                    _serverRoomApi = _serverApi.EnterRoomAsync(name, string.Empty, _cts.Token).Result;
                    if (_serverRoomApi != null)
                        RoomController.StartListenRoom(_serverRoomApi);
                });
            }
        }

        public void RunGameAsync()
        {
            if (_serverRoomApi == null)
                throw new ClientHostException("You are not in any of the rooms.");

            lock (_serverRoomApiLocker)
            {
                StartThread(() => _serverRoomApi.RunGatesGameAsync(_cts.Token));
            }
        }

        private Task StartThread(Action action)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    action();
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Error while process client command");
                }
            });
        }
    }
}
