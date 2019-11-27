using System;
using System.Threading;
using System.Threading.Tasks;
using Gates.Core.ServerApi;
using JetBrains.Annotations;

namespace Gates.ClientCore.Rooms
{
    [UsedImplicitly]
    internal class RoomController : IRoomController
    {
        public bool IsGameConnected { get; private set; }
        
        private const int ListenRequestPeriodMs = 250;
        private const int ListenRequestPeriodMsWhileRunnedGame = 2500;

        private IServerGatesApi _gameApi;
        private IServerRoomApi _roomApi;
        private bool _gameRunning;

        private Task _listenTask;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public IServerGatesApi GetActiveGameApi()
        {
            return _gameApi;
        }

        public void StartListenRoom([NotNull] IServerRoomApi roomApi)
        {
            if (roomApi == null) throw new ArgumentNullException(nameof(roomApi));

            lock (_roomApi)
            {
                if (_roomApi != null)
                    throw new ClientException("Room controller already listening one room.");

                _roomApi = roomApi;

                _listenTask = StartListenTask();
            }
        }

        public void Dispose()
        {
            using (_cts)
            {
                _cts.Cancel();
            }

            IsGameConnected = false;
            _gameApi = null;
            _listenTask.Dispose();
        }

        private Task StartListenTask()
        {
            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (true)
                    {
                        _cts.Token.ThrowIfCancellationRequested();

                        RoomState roomState = null;
                        try
                        {
                            roomState = _roomApi.GetState();
                        }
                        catch (Exception)
                        {
                            //TODO
                        }

                        _cts.Token.ThrowIfCancellationRequested();

                        if (roomState != null)
                        {
                            _gameRunning = ProcessRoomState(roomState);
                        }

                        _cts.Token.ThrowIfCancellationRequested();

                        var timeout = _gameRunning ? ListenRequestPeriodMsWhileRunnedGame : ListenRequestPeriodMs;
                        await Task.Delay(timeout).ConfigureAwait(true);
                    }
                }
                catch (OperationCanceledException)
                {
                    //ignored
                }
            }, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }

        private bool ProcessRoomState(RoomState roomState)
        {
            if (!roomState.GameStarted)
            {
                IsGameConnected = false;
                return false;
            }

            var connectedRunnedGame = _roomApi.ConnectToRunnedGame();
            _gameApi = connectedRunnedGame;
            IsGameConnected = true;
            return true;
        }
    }
}
