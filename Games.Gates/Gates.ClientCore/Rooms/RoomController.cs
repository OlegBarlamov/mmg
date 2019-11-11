using System;
using System.Threading;
using System.Threading.Tasks;
using Gates.Core.ServerApi;
using JetBrains.Annotations;

namespace Gates.ClientCore.Rooms
{
    internal class RoomController : IRoomController
    {
        private const int ListenRequestPeriodMs = 250;
        private const int ListenRequestPeriodMsWhileRunnedGame = 1500;

        private IServerRoomApi _roomApi;
        private bool _gameRunning;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public void StartListenRoom([NotNull] IServerRoomApi roomApi)
        {
            if (roomApi == null) throw new ArgumentNullException(nameof(roomApi));

            lock (_roomApi)
            {
                if (_roomApi != null)
                    throw new ClientException("Room controller already listening one room.");

                _roomApi = roomApi;

                StartListenTask();
            }
        }

        public void Dispose()
        {
            using (_cts)
            {
                _cts.Cancel();
            }

            //TODO
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
                return false;

            var connectedRunnedGame = _roomApi.ConnectToRunnedGame();

        }
    }
}
