using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Epic.Core.ClientMessages;
using Epic.Core.Logic;
using Epic.Core.Logic.Erros;
using Epic.Core.Objects.Battle;
using Epic.Core.Objects.BattleClientConnection;
using Epic.Core.ServerMessages;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Epic.Core.Services.GameManagement
{
    public class BattleGameManager : IBattleGameManager, IBattleMessageBroadcaster
    {
        public event Action<IBattleGameManager> Finished;
        public Guid BattleId => BattleObject.Id;
        
        private MutableBattleObject BattleObject { get; }
        private IBattleLogicFactory BattleLogicFactory { get; }

        private ILogger Logger { get; }
        
        [CanBeNull] private IBattleLogic _battleLogic;
        [CanBeNull] private CancellationTokenSource _battleLogicCancellationTokenSource;

        private bool _isDisposed;
        private bool _isPlaying;
        private readonly object _battleLogicLock = new object();
        private readonly object _clientConnectionsLock = new object();
        
        private readonly List<IBattleClientConnection> _clientConnections = new List<IBattleClientConnection>();
        
        public BattleGameManager(
            [NotNull] MutableBattleObject battleObject,
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IBattleLogicFactory battleLogicFactory)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            BattleObject = battleObject ?? throw new ArgumentNullException(nameof(battleObject));
            BattleLogicFactory = battleLogicFactory ?? throw new ArgumentNullException(nameof(battleLogicFactory));
            Logger = loggerFactory.CreateLogger<BattleGameManager>();
        }
        
        public void Dispose()
        {
            _isDisposed = true;
            Finished = null;
            if (_clientConnections.Count > 0)
            {
                Logger.LogError("There are client connections left in the BattleGameManager while disposing.");
            }
            SuspendBattle();
        }

        public int GetClientsCount()
        {
            lock (_clientConnectionsLock)
            {
                return _clientConnections.Count;
            }
        }

        public bool IsBattlePlaying()
        {
            return _isPlaying;
        }

        public void PlayBattle()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(BattleGameManager));
                
            if (IsBattlePlaying())
                return;

            lock (_battleLogicLock)
            {
                if (IsBattlePlaying())
                    return;

                _isPlaying = true;
                _battleLogic = CreateBattleLogic();
                _battleLogic!.Run(_battleLogicCancellationTokenSource!.Token).ContinueWith(t =>
                {
                    SuspendBattle();

                    var battleResult = t.Result;
                    if (battleResult.Finished)
                        Finished?.Invoke(this);
                });
            }
        }

        public void SuspendBattle()
        {
            _isPlaying = false;
            DestroyBattleLogic();
            StopAllConnections();
        }

        public Task AddClient(IBattleClientConnection connection)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(BattleGameManager));
            
            lock (_clientConnectionsLock)
            {
                _clientConnections.Add(connection);
                Logger.LogInformation($"Added battle {BattleId} connection {connection.ConnectionId}. Connections: {_clientConnections.Count}");
                
                connection.MessageReceived += ConnectionOnMessageReceived;
                if (_clientConnections.Count == 1)
                {
                    PlayBattle();
                }
            }
            return Task.CompletedTask;
        }
        
        public Task RemoveClient(IBattleClientConnection connection)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(BattleGameManager));
            
            lock (_clientConnectionsLock)
            {
                if (_clientConnections.Remove(connection))
                {
                    Logger.LogInformation(
                        $"Removed battle {BattleId} connection {connection.ConnectionId}. Connections: {_clientConnections.Count}");

                    connection.MessageReceived -= ConnectionOnMessageReceived;
                    if (_clientConnections.Count == 0)
                    {
                        SuspendBattle();
                    }
                }
            }
            return Task.CompletedTask;
        }
        
        public Task BroadcastMessageAsync(IServerBattleMessage message)
        {
            lock (_clientConnectionsLock)
            {
                return Task.WhenAll(
                    _clientConnections.Select(clientConnection => clientConnection.SendMessageAsync(message))
                );
            }
        }

        private void StopAllConnections()
        {
            lock (_clientConnectionsLock)
            {
                _clientConnections.ForEach(connection =>
                {
                    try
                    {
                        connection.CloseAsync().ContinueWith(t =>
                        {
                            if (t.IsFaulted)
                                Logger.LogError(t.Exception?.GetBaseException(), "Error while stopping battle connection.");
                        });
                        connection.Dispose();
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e, "Unexpected Error while stopping or disposing a battle connection while stopping all connections.");
                    }
                });
                _clientConnections.Clear();
            }
        }

        private IBattleLogic CreateBattleLogic()
        {
            if (_battleLogic != null)
            {
                Logger.LogError("Battle logic does already exist. Disposing.");
                DestroyBattleLogic();
            }

            _battleLogic = BattleLogicFactory.Create(BattleObject, this);
            _battleLogicCancellationTokenSource?.Dispose();
            _battleLogicCancellationTokenSource = new CancellationTokenSource();
            
            return _battleLogic;
        }

        private void DestroyBattleLogic()
        {
            lock (_battleLogicLock)
            {
                _battleLogicCancellationTokenSource?.Cancel();
                _battleLogicCancellationTokenSource?.Dispose();
                _battleLogicCancellationTokenSource = null;
                if (_battleLogic != null)
                {
                    _battleLogic.Dispose();
                    _battleLogic = null;
                }
            }
        }

        private async void ConnectionOnMessageReceived(IBattleClientConnection connection, IClientBattleMessage clientMessage)
        {
            try
            {
                Logger.LogInformation($"Message from {connection.ConnectionId}: {clientMessage.Command}");

                if (_battleLogic != null)
                {
                    await _battleLogic.OnClientMessage(connection, clientMessage);
                }
                else
                {
                    Logger.LogError($"Client message received, but the logic is suspended.");
                }
            }
            catch (ClientCommandRejected e)
            {
                if (connection.IsConnected)
                    await connection.SendMessageAsync(new CommandRejected(clientMessage, e.GetType().Name, e.Message));
            }
            catch (Exception e)
            {
                Logger.LogError($"Error while processing the client message: {clientMessage}. {e.Message}");
                if (connection.IsConnected)
                    await connection.SendMessageAsync(new CommandRejected(clientMessage, "Unknown reason", e.Message));
            }
        }
    }
}