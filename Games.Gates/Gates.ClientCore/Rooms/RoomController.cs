using System;
using System.Threading;
using System.Threading.Tasks;
using Gates.ClientCore.Logging;
using Gates.Core.ServerApi;
using JetBrains.Annotations;
using Logging;
using Microsoft.Extensions.Logging;

namespace Gates.ClientCore.Rooms
{
    [UsedImplicitly]
    internal class RoomController : IRoomController
    {
        public bool IsGameConnected { get; private set; }
        
        private ILogger Logger { get; }

        private const int ListenRequestPeriodMs = 250;
        private const int ListenRequestPeriodMsWhileRunnedGame = 2500;

        private IServerGatesApi _gameApi;
        private IServerRoomApi _roomApi;
        private bool _gameRunning;

        private Task _listenTask;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public RoomController(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger(LogCategories.Room);
        }

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
                            roomState = await _roomApi.GetStateAsync(_cts.Token).ConfigureAwait(true);
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e, "Error while get room state");
                        }

                        _cts.Token.ThrowIfCancellationRequested();

                        if (roomState != null)
                        {
                            _gameRunning = await ProcessRoomStateAsync(roomState, _cts.Token).ConfigureAwait(true);
                        }

                        _cts.Token.ThrowIfCancellationRequested();

                        var timeout = _gameRunning ? ListenRequestPeriodMsWhileRunnedGame : ListenRequestPeriodMs;
                        await Task.Delay(timeout).ConfigureAwait(true);
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Unknown error while listem the room");
                }
            }, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }

        private async Task<bool> ProcessRoomStateAsync(RoomState roomState, CancellationToken cancellationToken)
        {
            if (!roomState.GameStarted)
            {
                IsGameConnected = false;
                return false;
            }

            var connectedRunnedGame = await _roomApi.ConnectToRunnedGameAsync(cancellationToken)
                .ConfigureAwait(true);

            _gameApi = connectedRunnedGame;
            IsGameConnected = true;
            return true;
        }
    }
}
