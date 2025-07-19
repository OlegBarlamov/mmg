using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Epic.Core;
using Epic.Core.ClientMessages;
using Epic.Core.Logic;
using Epic.Core.Logic.Erros;
using Epic.Core.Objects.Battle;
using Epic.Core.Objects.BattleClientConnection;
using Epic.Core.Objects.BattleUnit;
using Epic.Core.ServerMessages;
using Epic.Core.Services.Battles;
using Epic.Core.Services.GameManagement;
using Epic.Core.Services.Rewards;
using Epic.Core.Services.Units;
using JetBrains.Annotations;

namespace Epic.Logic
{
    public class BattleLogic : IBattleLogic
    {
        private MutableBattleObject BattleObject { get; }
        private IBattleUnitsService BattleUnitsService { get; }
        private IPlayerUnitsService PlayerUnitsService { get; }
        private IBattlesService BattlesService { get; }
        private IRewardsService RewardsService { get; }
        private IBattleMessageBroadcaster Broadcaster { get; }

        private readonly List<MutableBattleUnitObject> _sortedBattleUnitObjects;

        private readonly ConcurrentDictionary<int, List<IServerBattleMessage>> _passedServerBattleMessages = new ConcurrentDictionary<int, List<IServerBattleMessage>>();

        [CanBeNull] private TurnInfo _expectedTurn;
        [CanBeNull] private TaskCompletionSource<Task> _awaitPlayerTurnTaskCompletionSource;
        private bool _isDisposed;
        
        public BattleLogic(
            [NotNull] MutableBattleObject battleObject, 
            [NotNull] IBattleUnitsService battleUnitsService,
            [NotNull] IPlayerUnitsService playerUnitsService,
            [NotNull] IBattlesService battlesService,
            [NotNull] IRewardsService rewardsService,
            [NotNull] IBattleMessageBroadcaster broadcaster)
        {
            BattleObject = battleObject ?? throw new ArgumentNullException(nameof(battleObject));
            BattleUnitsService = battleUnitsService ?? throw new ArgumentNullException(nameof(battleUnitsService));
            PlayerUnitsService = playerUnitsService ?? throw new ArgumentNullException(nameof(playerUnitsService));
            BattlesService = battlesService ?? throw new ArgumentNullException(nameof(battlesService));
            RewardsService = rewardsService ?? throw new ArgumentNullException(nameof(rewardsService));
            Broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));

            _sortedBattleUnitObjects = new List<MutableBattleUnitObject>(battleObject.Units);
            _sortedBattleUnitObjects.Sort((x, y) => x.PlayerUnit.UnitType.Speed.CompareTo(y.PlayerUnit.UnitType.Speed));
        }
        
        public void Dispose()
        {
            _isDisposed = true;
            _sortedBattleUnitObjects.Clear();
            _awaitPlayerTurnTaskCompletionSource?.TrySetCanceled();
            _passedServerBattleMessages.Clear();
        }

        public async Task<BattleResult> Run(CancellationToken cancellationToken)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(BattleLogic));
                
            var activeUnit = GetActiveUnit(BattleObject.TurnIndex);
            var battleResult = GetBattleResult();

            try
            {
                while (!battleResult.Finished)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (IsPlayerControlled(activeUnit))
                    {
                        await WaitForClientTurn(activeUnit.PlayerIndex, BattleObject.TurnIndex);
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                    else
                    {
                        // TODO AI
                    }

                    BattleObject.TurnIndex++;
                    await BattlesService.UpdateBattle(BattleObject);

                    battleResult = GetBattleResult();
                    if (battleResult.Finished)
                        continue;
                    
                    activeUnit = GetActiveUnit(BattleObject.TurnIndex);
                    var serverCommand = new NextTurnCommandFromServer(BattleObject.TurnIndex, (InBattlePlayerNumber)activeUnit.PlayerIndex);
                    await BroadcastMessageToClientAndSaveAsync(serverCommand);
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
            }

            if (battleResult.Finished)
                await OnBattleFinished(battleResult);
            
            return battleResult;
        }

        private async Task OnBattleFinished(BattleResult battleResult)
        {
            if (battleResult.Winner != null)
            {
                var winnerUserId = BattleObject.GetPlayerId(battleResult.Winner.Value);
                if (winnerUserId.HasValue)
                {
                    // TODO ignore AI player
                    var rewards =
                        await RewardsService.GetRewardsFromBattleDefinition(BattleObject.BattleDefinitionId);
                    var rewardsIds = rewards.Select(x => x.Id).ToArray();
                    await RewardsService.GiveRewardsToPlayerAsync(rewardsIds, winnerUserId.Value);
                    
                    
                }
            }

            var battleFinishedCommand = new BattleFinishedCommandFromServer(BattleObject.TurnIndex)
            {
                Winner = battleResult.Winner?.ToString() ?? string.Empty,
            };
            await BroadcastMessageToClientAndSaveAsync(battleFinishedCommand);

            await BattlesService.FinishBattle(BattleObject, battleResult);
        }

        private Task BroadcastMessageToClientAndSaveAsync(IServerBattleMessage message)
        {
            var turnMessages = _passedServerBattleMessages.GetOrAdd(message.TurnNumber, _ => new List<IServerBattleMessage>());
            turnMessages.Add(message);
            return Broadcaster.BroadcastMessageAsync(message);
        }

        private BattleResult GetBattleResult()
        {
            var noAliveUnits = true;
            InBattlePlayerNumber? winner = null;
            foreach (var battleUnitObject in _sortedBattleUnitObjects)
            {
                if (battleUnitObject.PlayerUnit.IsAlive)
                {
                    var player = (InBattlePlayerNumber)battleUnitObject.PlayerIndex;
                    noAliveUnits = false;
                    if (winner == null)
                        winner = player;
                    else if (player != winner)
                    {
                        winner = null;
                        break;
                    }
                        
                }
            }

            return new BattleResult
            {
                Finished = noAliveUnits || winner.HasValue,
                Winner = winner
            };
        }

        private bool IsPlayerControlled(IBattleUnitObject unit)
        {
            // TODO change to add AI
            return true;
        }

        private Task WaitForClientTurn(int playerIndex, int turnIndex)
        {
            _expectedTurn = new TurnInfo(turnIndex, playerIndex);
            _awaitPlayerTurnTaskCompletionSource = new TaskCompletionSource<Task>();
            return _awaitPlayerTurnTaskCompletionSource.Task;
        }

        public Task OnClientMessage(IBattleClientConnection connection, IClientBattleMessage clientBattleMessage)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(BattleLogic));
            
            switch (clientBattleMessage.Command)
            {
                case ClientBattleCommands.CLIENT_CONNECTED:
                    return OnClientConnected(connection, (ClientConnectedBattleMessage)clientBattleMessage);
                case ClientBattleCommands.UNIT_MOVE:
                    return OnClientUnitMove((UnitMoveClientBattleMessage)clientBattleMessage);
                case ClientBattleCommands.UNIT_ATTACK:
                    return OnClientUnitAttack((UnitAttackClientBattleMessage)clientBattleMessage);
                default:
                    throw new ClientCommandRejected("Unknown client command");
            }
        }

        private async Task OnClientUnitAttack(UnitAttackClientBattleMessage command)
        {
            var targetActor =
                BattleObject.Units.FirstOrDefault(x => x.Id.ToString() == command.ActorId);
            if (targetActor == null)
                throw new BattleLogicException("Not found target actor for client command");

            var targetTarget = BattleObject.Units.FirstOrDefault(x => x.Id.ToString() == command.TargetId);
            if (targetTarget == null)
                throw new BattleLogicException("Not found target unit for client command");

            if (command.TurnIndex != _expectedTurn?.TurnIndex || (int)command.Player != _expectedTurn?.PlayerIndex)
                throw new BattleLogicException("Wrong turn index or player index");

            var mutableActor = targetActor;
            mutableActor.Column = command.MoveToCell.C;
            mutableActor.Row = command.MoveToCell.R;

            //TODO Check if it is reachable 

            await BattleUnitsService.UpdateUnits(new[] { mutableActor });
            
            await BroadcastMessageToClientAndSaveAsync(
                new UnitMoveCommandFromServer(command.TurnIndex, command.Player, command.ActorId, command.MoveToCell)
                );
            
            await BroadcastMessageToClientAndSaveAsync(
                new UnitAttackCommandFromServer(command.TurnIndex, command.Player, command.ActorId, command.TargetId)
                );
            
            var unitTakesDamageData = UnitTakesDamageData.FromUnitAndTarget(targetActor, targetTarget);
            targetTarget.PlayerUnit.Count = unitTakesDamageData.RemainingCount;
            targetTarget.PlayerUnit.IsAlive = targetTarget.PlayerUnit.Count > 0;

            await PlayerUnitsService.UpdateUnits(new [] { targetTarget.PlayerUnit });

            targetTarget.CurrentHealth = unitTakesDamageData.RemainingHealth;
            
            await BattleUnitsService.UpdateUnits(new[] { targetTarget });
                
            var serverUnitTakesDamage = new UnitTakesDamageCommandFromServer(command.TurnIndex, command.Player, command.TargetId)
            {
                DamageTaken = unitTakesDamageData.DamageTaken,
                KilledCount = unitTakesDamageData.KilledCount,
                RemainingCount = unitTakesDamageData.RemainingCount,
                RemainingHealth = unitTakesDamageData.RemainingHealth,
            };
            await BroadcastMessageToClientAndSaveAsync(serverUnitTakesDamage);
            
            _awaitPlayerTurnTaskCompletionSource?.SetResult(null);
        }

        private async Task OnClientUnitMove(UnitMoveClientBattleMessage command)
        {
            var targetActor =
                BattleObject.Units.FirstOrDefault(x => x.Id.ToString() == command.ActorId);
            if (targetActor == null)
                throw new BattleLogicException("Not found target actor for client command");

            if (command.TurnIndex != _expectedTurn?.TurnIndex || (int)command.Player != _expectedTurn?.PlayerIndex) 
                throw new BattleLogicException("Wrong turn index or player index");
            
            var mutableActor = targetActor;
            mutableActor.Column = command.MoveToCell.C;
            mutableActor.Row = command.MoveToCell.R;
            
            //TODO Check if it is reachable 
            
            await BattleUnitsService.UpdateUnits(new[] { mutableActor });

            var serverCommand = new UnitMoveCommandFromServer(command.TurnIndex, command.Player, command.ActorId,
                command.MoveToCell);
            await BroadcastMessageToClientAndSaveAsync(serverCommand);
            
            _awaitPlayerTurnTaskCompletionSource?.SetResult(null);
        }

        private async Task OnClientConnected(IBattleClientConnection connection, ClientConnectedBattleMessage message)
        {
            for (int i = message.TurnIndex; i <= BattleObject.TurnIndex; i++)
            {
                if (_passedServerBattleMessages.TryGetValue(i, out var messagesFromTurn))
                {
                    await Task.WhenAll(messagesFromTurn.Select(connection.SendMessageAsync));
                }
            }
        }

        private IBattleUnitObject GetActiveUnit(int turnIndex)
        {
            turnIndex %= _sortedBattleUnitObjects.Count;
            var activeUnit = _sortedBattleUnitObjects[turnIndex];
            while (!activeUnit.PlayerUnit.IsAlive)
            {
                turnIndex++;
                turnIndex %= _sortedBattleUnitObjects.Count;
                activeUnit = _sortedBattleUnitObjects[turnIndex];
            }
            return activeUnit;
        }
    }
}