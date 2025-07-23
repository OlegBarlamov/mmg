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
using Epic.Core.ServerMessages;
using Epic.Core.Services.Battles;
using Epic.Core.Services.Connection;
using Epic.Core.Services.GameManagement;
using Epic.Core.Services.Players;
using Epic.Core.Services.Rewards;
using Epic.Core.Services.Units;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Epic.Logic
{
    public class BattleLogic : IBattleLogic
    {
        private MutableBattleObject BattleObject { get; }
        private IBattleUnitsService BattleUnitsService { get; }
        private IGlobalUnitsService GlobalUnitsService { get; }
        private IBattlesService BattlesService { get; }
        private IRewardsService RewardsService { get; }
        private IBattleMessageBroadcaster Broadcaster { get; }
        private IDaysProcessor DaysProcessor { get; }
        private IPlayersService PlayersService { get; }
        private ILogger<BattleLogic> Logger { get; }
        public IRandomProvider RandomProvider { get; }

        private readonly List<MutableBattleUnitObject> _sortedBattleUnitObjects;

        private readonly ConcurrentDictionary<int, List<IServerBattleMessage>> _passedServerBattleMessages = new ConcurrentDictionary<int, List<IServerBattleMessage>>();

        private IBattleUnitObject _activeUnit;
        [CanBeNull] private TurnInfo _expectedTurn;
        [CanBeNull] private TaskCompletionSource<Task> _awaitPlayerTurnTaskCompletionSource;
        private bool _isDisposed;
        
        public BattleLogic(
            [NotNull] MutableBattleObject battleObject, 
            [NotNull] IBattleUnitsService battleUnitsService,
            [NotNull] IGlobalUnitsService globalUnitsService,
            [NotNull] IBattlesService battlesService,
            [NotNull] IRewardsService rewardsService,
            [NotNull] IBattleMessageBroadcaster broadcaster,
            [NotNull] IDaysProcessor daysProcessor,
            [NotNull] IPlayersService playersService,
            [NotNull] ILogger<BattleLogic> logger,
            [NotNull] IRandomProvider randomProvider)
        {
            BattleObject = battleObject ?? throw new ArgumentNullException(nameof(battleObject));
            BattleUnitsService = battleUnitsService ?? throw new ArgumentNullException(nameof(battleUnitsService));
            GlobalUnitsService = globalUnitsService ?? throw new ArgumentNullException(nameof(globalUnitsService));
            BattlesService = battlesService ?? throw new ArgumentNullException(nameof(battlesService));
            RewardsService = rewardsService ?? throw new ArgumentNullException(nameof(rewardsService));
            Broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
            DaysProcessor = daysProcessor ?? throw new ArgumentNullException(nameof(daysProcessor));
            PlayersService = playersService ?? throw new ArgumentNullException(nameof(playersService));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            RandomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider));

            _sortedBattleUnitObjects = new List<MutableBattleUnitObject>(battleObject.Units);
            _sortedBattleUnitObjects.Sort((x, y) => y.GlobalUnit.UnitType.Speed.CompareTo(x.GlobalUnit.UnitType.Speed));
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
                
            var battleResult = GetBattleResult();
            try
            {
                while (!battleResult.Finished)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    BattleObject.TurnNumber++;
                    await BattlesService.UpdateBattle(BattleObject);

                    cancellationToken.ThrowIfCancellationRequested();

                    _activeUnit = GetActiveUnit(BattleObject.LastTurnUnitIndex, out var activeUnitIndex);
                    BattleObject.LastTurnUnitIndex = activeUnitIndex;
                    await BattlesService.UpdateBattle(BattleObject);
                    
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    await BroadcastMessageToClientAndSaveAsync(new NextTurnCommandFromServer(
                        BattleObject.TurnNumber,
                        (InBattlePlayerNumber)_activeUnit.PlayerIndex,
                        _activeUnit.Id));

                    cancellationToken.ThrowIfCancellationRequested();

                    if (IsHumanControlled(_activeUnit))
                    {
                        await WaitForClientTurn(_activeUnit.PlayerIndex, BattleObject.TurnNumber);
                    }
                    else
                    {
                        // TODO AI
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    battleResult = GetBattleResult();
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (Exception e)
            {
                Logger.LogError($"Unexpected error in battle logic cycle: {e}");
            }

            if (battleResult.Finished)
                await OnBattleFinished(battleResult);
            
            return battleResult;
        }

        private async Task OnBattleFinished(BattleResult battleResult)
        {
            var winnerPlayerId = Guid.Empty;
            if (battleResult.Winner != null)
            {
                var winnerId = BattleObject.GetPlayerId(battleResult.Winner.Value);
                if (winnerId.HasValue)
                {
                    winnerPlayerId = winnerId.Value;
                    var rewards = await RewardsService.GetRewardsFromBattleDefinition(BattleObject.BattleDefinitionId);
                    var rewardsIds = rewards.Select(x => x.Id).ToArray();
                    await RewardsService.GiveRewardsToPlayerAsync(rewardsIds, winnerPlayerId);
                }
            }
            
            var defeatedPlayers = BattleObject.PlayerIds.Where(x => x != winnerPlayerId).ToArray();
            await PlayersService.SetDefeated(defeatedPlayers);

            await DaysProcessor.ProcessNewDay(BattleObject.PlayerIds.ToArray());

            var battleFinishedCommand = new BattleFinishedCommandFromServer(BattleObject.TurnNumber)
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
                if (battleUnitObject.GlobalUnit.IsAlive)
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

        private bool IsHumanControlled(IBattleUnitObject unit)
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
                    return OnClientUnitMove(connection, (UnitMoveClientBattleMessage)clientBattleMessage);
                case ClientBattleCommands.UNIT_ATTACK:
                    return OnClientUnitAttack(connection, (UnitAttackClientBattleMessage)clientBattleMessage);
                default:
                    throw new ClientCommandRejected("Unknown client command");
            }
        }

        private async Task OnClientUnitAttack(IBattleClientConnection connection, UnitAttackClientBattleMessage command)
        {
            var targetActor =
                BattleObject.Units.FirstOrDefault(x => x.Id.ToString() == command.ActorId);
            if (targetActor == null)
                throw new BattleLogicException("Not found target actor for client command");
            if (targetActor != _activeUnit)
                throw new BattleLogicException("Wrong target actor for client command");
                
            var targetTarget = BattleObject.Units.FirstOrDefault(x => x.Id.ToString() == command.TargetId);
            if (targetTarget == null)
                throw new BattleLogicException("Not found target unit for client command");

            if (command.TurnIndex != _expectedTurn?.TurnIndex || (int)command.Player != _expectedTurn?.PlayerIndex)
                throw new BattleLogicException("Wrong turn index or player index");

            var availableAttacks = targetActor.GlobalUnit.UnitType.Attacks; 
            if (command.AttackIndex < 0 || command.AttackIndex >= availableAttacks.Count)
                throw new BattleLogicException($"Wrong Attack Type index {command.AttackIndex}");
            var attackFunction = availableAttacks[command.AttackIndex];
            if (attackFunction.StayOnly && command.MoveToCell != new HexoPoint(targetActor.Column, targetActor.Row))
                throw new BattleLogicException($"The target Attack Type {attackFunction.Name} does not allow moving");
            var range = OddRHexoGrid.Distance(command.MoveToCell, targetTarget);
            if (range < attackFunction.AttackMinRange || range > attackFunction.AttackMaxRange)
                throw new BattleLogicException("The target is out of range for attack");
            if (attackFunction.EnemyInRangeDisablesAttack > 0 && Utils.IsEnemyInRange(targetActor, attackFunction.EnemyInRangeDisablesAttack,
                    _sortedBattleUnitObjects))
                throw new BattleLogicException($"The attack is blocked by an enemy in range {attackFunction.EnemyInRangeDisablesAttack}");
                
                
            var mutableActor = targetActor;
            mutableActor.Column = command.MoveToCell.C;
            mutableActor.Row = command.MoveToCell.R;

            //TODO Check if it is reachable 
            
            await connection.SendMessageAsync(new CommandApproved(command));

            await BattleUnitsService.UpdateUnits(new[] { mutableActor });
            
            await BroadcastMessageToClientAndSaveAsync(
                new UnitMoveCommandFromServer(command.TurnIndex, command.Player, command.ActorId, command.MoveToCell)
                );
            
            await BroadcastMessageToClientAndSaveAsync(
                new UnitAttackCommandFromServer(command.TurnIndex, command.Player, command.ActorId, command.TargetId)
                );
            
            var unitTakesDamageData = UnitTakesDamageData.FromUnitAndTarget(
                targetActor, 
                targetTarget,
                attackFunction,
                range,
                false,
                RandomProvider.Random);
            
            targetTarget.GlobalUnit.Count = unitTakesDamageData.RemainingCount;
            targetTarget.GlobalUnit.IsAlive = targetTarget.GlobalUnit.Count > 0;

            await GlobalUnitsService.UpdateUnits(new [] { targetTarget.GlobalUnit });

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

        private async Task OnClientUnitMove(IBattleClientConnection connection, UnitMoveClientBattleMessage command)
        {
            var targetActor =
                BattleObject.Units.FirstOrDefault(x => x.Id.ToString() == command.ActorId);
            if (targetActor == null)
                throw new BattleLogicException("Not found target actor for client command");
            if (targetActor != _activeUnit)
                throw new BattleLogicException("Wrong target actor for client command");
            
            if (command.TurnIndex != _expectedTurn?.TurnIndex || (int)command.Player != _expectedTurn?.PlayerIndex) 
                throw new BattleLogicException("Wrong turn index or player index");
            
            var mutableActor = targetActor;
            mutableActor.Column = command.MoveToCell.C;
            mutableActor.Row = command.MoveToCell.R;
            
            //TODO Check if it is reachable 

            await connection.SendMessageAsync(new CommandApproved(command));
            
            await BattleUnitsService.UpdateUnits(new[] { mutableActor });

            var serverCommand = new UnitMoveCommandFromServer(command.TurnIndex, command.Player, command.ActorId,
                command.MoveToCell);
            await BroadcastMessageToClientAndSaveAsync(serverCommand);
            
            _awaitPlayerTurnTaskCompletionSource?.SetResult(null);
        }

        private async Task OnClientConnected(IBattleClientConnection connection, ClientConnectedBattleMessage message)
        {
            await connection.SendMessageAsync(new CommandApproved(message));
            
            for (int i = message.TurnIndex; i <= BattleObject.TurnNumber; i++)
            {
                if (_passedServerBattleMessages.TryGetValue(i, out var messagesFromTurn))
                {
                    await Task.WhenAll(messagesFromTurn.Select(connection.SendMessageAsync));
                }
            }
        }

        private IBattleUnitObject GetActiveUnit(int lastTurnUnitIndex, out int activeUnitIndex)
        {
            int count = _sortedBattleUnitObjects.Count;

            for (int i = 1; i <= count; i++)
            {
                int nextIndex = (lastTurnUnitIndex + i) % count;
                var unit = _sortedBattleUnitObjects[nextIndex];

                if (unit.GlobalUnit.IsAlive)
                {
                    activeUnitIndex = nextIndex;
                    return unit;
                }
            }

            throw new BattleLogicException("No alive unit found for active unit");
        }
    }
}