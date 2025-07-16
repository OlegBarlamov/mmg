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

namespace Epic.Core.Objects.BattleGameManager
{
    public class BattleGameManager : IBattleGameManager
    {
        public event Action<IBattleGameManager> Finished;
        public bool IsDisposed { get; private set; }
        public Guid BattleId => BattleObject.Id;
        private MutableBattleObject BattleObject { get; set; }
        private IBattleUnitsService BattleUnitsService { get; }
        private IUserUnitsService UserUnitsService { get; }
        private IBattlesService BattlesService { get; }
        private IRewardsService RewardsService { get; }

        private ILogger Logger { get; }
        
        private bool _isPlaying;
        private IBattleLogic _battleLogic;
        private readonly object _battleLogicLock = new object();
        private CancellationTokenSource _battleLogicCancellationTokenSource;
        
        private readonly object _clientConnectionsLock = new object();
        private readonly List<IBattleClientConnection> _clientConnections = new List<IBattleClientConnection>();
        
        public BattleGameManager(
            [NotNull] MutableBattleObject battleObject,
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IBattleUnitsService battleUnitsService,
            [NotNull] IUserUnitsService userUnitsService,
            [NotNull] IBattlesService battlesService,
            [NotNull] IRewardsService rewardsService)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            BattleObject = battleObject ?? throw new ArgumentNullException(nameof(battleObject));
            BattleUnitsService = battleUnitsService ?? throw new ArgumentNullException(nameof(battleUnitsService));
            UserUnitsService = userUnitsService ?? throw new ArgumentNullException(nameof(userUnitsService));
            BattlesService = battlesService ?? throw new ArgumentNullException(nameof(battlesService));
            RewardsService = rewardsService ?? throw new ArgumentNullException(nameof(rewardsService));
            Logger = loggerFactory.CreateLogger<BattleGameManager>();
        }
        
        public void Dispose()
        {
            IsDisposed = true;
            Finished = null;
            SuspendBattle();
            _battleLogic.Dispose();
            if (_clientConnections.Count > 0)
            {
                Logger.LogError("There are client connections left in the BattleGameManager while disposing.");
            }
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
            if (IsBattlePlaying())
                return;

            lock (_battleLogicLock)
            {
                if (IsBattlePlaying())
                    return;

                _isPlaying = true;
                _battleLogic ??= new BattleLogic(
                    BattleObject,
                    BattleUnitsService,
                    UserUnitsService,
                    BattlesService,
                    RewardsService);
                
                _battleLogic.BroadcastMessage += BattleLogicOnBroadcastMessage;
                _battleLogicCancellationTokenSource?.Dispose();
                _battleLogicCancellationTokenSource = new CancellationTokenSource();
            }

            _battleLogic.Run(_battleLogicCancellationTokenSource.Token).ContinueWith(t =>
            {
                var battleResult = t.Result;
                if (battleResult.Finished)
                {
                    SuspendBattle();
                    Finished?.Invoke(this);
                }
            });

        }

        private void BattleLogicOnBroadcastMessage(IServerBattleMessage message)
        {
            lock (_clientConnectionsLock)
            {
                Task.WhenAll(
                    _clientConnections.Select(clientConnection => clientConnection.SendMessageAsync(message))
                );
            }
        }

        public void SuspendBattle()
        {
            _isPlaying = false;
            _battleLogicCancellationTokenSource?.Cancel();
        }

        public Task AddClient(IBattleClientConnection connection)
        {
            lock (_clientConnectionsLock)
            {
                _clientConnections.Add(connection);
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
            lock (_clientConnectionsLock)
            {
                _clientConnections.Remove(connection);
                connection.MessageReceived -= ConnectionOnMessageReceived;
                if (_clientConnections.Count == 0)
                {
                    SuspendBattle();
                }
            }
            return Task.CompletedTask;
        }

        public Task StopAllConnections()
        {
            throw new NotImplementedException();
        }

        private async void ConnectionOnMessageReceived(IBattleClientConnection connection, IClientBattleMessage clientMessage)
        {
            try
            {
                Logger.LogInformation($"Message from {connection.ConnectionId}: {clientMessage.Command}");
                
                if (_battleLogic != null)
                {
                    await _battleLogic.OnClientMessage(connection, clientMessage);
                    await connection.SendMessageAsync(new CommandApproved(clientMessage)
                    {
                        CommandId = Guid.NewGuid().ToString(),
                        TurnNumber = clientMessage.TurnIndex,
                    });
                }
                else
                {
                    Logger.LogError($"Client message received, but the logic is suspended.");
                }
            }
            catch (ClientCommandRejected e)
            {
                await connection.SendMessageAsync(new CommandRejected(clientMessage, e.GetType().Name, e.Message)
                {
                    CommandId = Guid.NewGuid().ToString(),
                    TurnNumber = clientMessage.TurnIndex,
                });
            }
            catch (Exception e)
            {
                Logger.LogError($"Error while processing the client message: {clientMessage}. {e.Message}");
                await connection.SendMessageAsync(new CommandRejected(clientMessage, "Unknown reason", e.Message)
                {
                    CommandId = Guid.NewGuid().ToString(),
                    TurnNumber = clientMessage.TurnIndex,
                });
            }
        }
    }
}