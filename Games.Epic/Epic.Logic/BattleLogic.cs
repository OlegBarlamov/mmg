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
using NetExtensions.Collections;

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
        private IRandomProvider RandomProvider { get; }

        private readonly List<MutableBattleUnitObject> _sortedBattleUnitObjects;

        private readonly ConcurrentDictionary<int, List<IServerBattleMessage>> _passedServerBattleMessages = new ConcurrentDictionary<int, List<IServerBattleMessage>>();

        private IBattleUnitObject _activeUnit;
        [CanBeNull] private TurnInfo _expectedTurn;
        [CanBeNull] private TaskCompletionSource<ClientUserAction> _awaitPlayerTurnTaskCompletionSource;
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
            SortByInitiative(_sortedBattleUnitObjects);
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

            if (!battleResult.Finished && BattleObject.TurnNumber < 0)
            {
                // Initialize the battle
                BattleObject.TurnNumber = 0;
                await BattlesService.UpdateBattle(BattleObject);
                
                cancellationToken.ThrowIfCancellationRequested();
            }

            try
            {
                while (!battleResult.Finished)
                {
                    _activeUnit = GetActiveUnit(BattleObject.LastTurnUnitIndex, out var activeUnitIndex);
                    await BroadcastMessageToClientAndSaveAsync(new NextTurnCommandFromServer(
                        BattleObject.TurnNumber,
                        (InBattlePlayerNumber)_activeUnit.PlayerIndex,
                        _activeUnit.Id,
                        BattleObject.RoundNumber));
                    
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    if (IsHumanControlled(_activeUnit))
                    {
                        var userAction = await WaitForClientTurn(_activeUnit.PlayerIndex, BattleObject.TurnNumber);
                        if (userAction.CommandName == ClientBattleCommands.UNIT_WAIT)
                            activeUnitIndex--;
                    }
                    else
                    {
                        // TODO AI
                    }

                    var isNextRound = activeUnitIndex + 1 >= _sortedBattleUnitObjects.Count;
                    if (isNextRound)
                    {
                        BattleObject.RoundNumber++;
                        _sortedBattleUnitObjects.ForEach(unit =>
                        {
                            unit.Waited = false;
                            unit.AttackFunctionsData.ForEach(x => x.CounterattacksUsed = 0);
                        });
                        await BattleUnitsService.UpdateUnits(_sortedBattleUnitObjects);
                        SortByInitiative(_sortedBattleUnitObjects);
                    }
                    BattleObject.LastTurnUnitIndex = activeUnitIndex;
                    BattleObject.TurnNumber++;
                    await BattlesService.UpdateBattle(BattleObject);
                    
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
            // TODO kill the heroes

            if (BattleObject.ProgressDays)
                await DaysProcessor.ProcessNewDay(BattleObject.PlayerIds.ToArray());

            var reportEntity = await BattlesService.FinishBattle(BattleObject, battleResult);
            
            var battleFinishedCommand = new BattleFinishedCommandFromServer(BattleObject.TurnNumber)
            {
                Winner = battleResult.Winner?.ToString() ?? string.Empty,
                ReportId = reportEntity.Id.ToString(),
            };
            await BroadcastMessageToClientAndSaveAsync(battleFinishedCommand);
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

        private Task<ClientUserAction> WaitForClientTurn(int playerIndex, int turnIndex)
        {
            _expectedTurn = new TurnInfo(turnIndex, playerIndex);
            _awaitPlayerTurnTaskCompletionSource = new TaskCompletionSource<ClientUserAction>();
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
                case ClientBattleCommands.UNIT_PASS:
                    return OnClientUnitPasses(connection, (UnitPassClientBattleMessage)clientBattleMessage);
                case ClientBattleCommands.UNIT_WAIT:
                    return OnClientUnitWaits(connection, (UnitWaitClientBattleMessage)clientBattleMessage);
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
            if (targetActor.AttackFunctionsData[command.AttackIndex].BulletsCount < 1)
                throw new BattleLogicException($"The attack does not have enough bullets {attackFunction.EnemyInRangeDisablesAttack}");
                
                
            var mutableActor = targetActor;
            mutableActor.Column = command.MoveToCell.C;
            mutableActor.Row = command.MoveToCell.R;
            mutableActor.AttackFunctionsData[command.AttackIndex].BulletsCount -= 1;
            
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

            targetTarget.CurrentCount = unitTakesDamageData.RemainingCount;
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

            if (attackFunction.CanTargetCounterattack && targetTarget.GlobalUnit.IsAlive)
            {
                var attackFunctionForCounterattack = targetTarget.AttackFunctionsData.FirstOrDefault(x =>
                    {
                        var counterattackFunctionType = targetTarget.GlobalUnit.UnitType.Attacks[x.AttackIndex];
                        var enoughAttacks = counterattackFunctionType.CounterattacksCount - x.CounterattacksUsed > 0;
                        return enoughAttacks && x.BulletsCount > 0 &&
                               range >= counterattackFunctionType.AttackMinRange &&
                               range <= counterattackFunctionType.AttackMaxRange &&
                               (counterattackFunctionType.EnemyInRangeDisablesAttack <= 0 ||
                                !Utils.IsEnemyInRange(targetTarget,
                                    counterattackFunctionType.EnemyInRangeDisablesAttack,
                                    _sortedBattleUnitObjects));
                    });

                if (attackFunctionForCounterattack != null)
                {
                    await BroadcastMessageToClientAndSaveAsync(
                        new UnitAttackCommandFromServer(command.TurnIndex, command.Player, command.TargetId, command.ActorId)
                    );

                    attackFunctionForCounterattack.CounterattacksUsed++;
                    attackFunctionForCounterattack.BulletsCount--;
                    await BattleUnitsService.UpdateUnits(new [] { targetTarget });
                
                    unitTakesDamageData = UnitTakesDamageData.FromUnitAndTarget(
                        targetTarget,
                        targetActor, 
                        targetTarget.GlobalUnit.UnitType.Attacks[attackFunctionForCounterattack.AttackIndex],
                        range,
                        true,
                        RandomProvider.Random);
            
                    targetActor.GlobalUnit.Count = unitTakesDamageData.RemainingCount;
                    targetActor.GlobalUnit.IsAlive = targetActor.GlobalUnit.Count > 0;

                    await GlobalUnitsService.UpdateUnits(new [] { targetActor.GlobalUnit });

                    targetActor.CurrentCount = unitTakesDamageData.RemainingCount;
                    targetActor.CurrentHealth = unitTakesDamageData.RemainingHealth;
            
                    await BattleUnitsService.UpdateUnits(new[] { targetActor });
                
                    serverUnitTakesDamage = new UnitTakesDamageCommandFromServer(command.TurnIndex, command.Player, command.ActorId)
                    {
                        DamageTaken = unitTakesDamageData.DamageTaken,
                        KilledCount = unitTakesDamageData.KilledCount,
                        RemainingCount = unitTakesDamageData.RemainingCount,
                        RemainingHealth = unitTakesDamageData.RemainingHealth,
                    };
                    await BroadcastMessageToClientAndSaveAsync(serverUnitTakesDamage);
                }
            }
            
            _awaitPlayerTurnTaskCompletionSource?.SetResult(new ClientUserAction(command.Command));
        }

        private async Task OnClientUnitPasses(IBattleClientConnection connection, UnitPassClientBattleMessage command)
        {
            var targetActor =
                BattleObject.Units.FirstOrDefault(x => x.Id.ToString() == command.ActorId);
            if (targetActor == null)
                throw new BattleLogicException("Not found target actor for client command");
            if (targetActor != _activeUnit)
                throw new BattleLogicException("Wrong target actor for client command");
            
            if (command.TurnIndex != _expectedTurn?.TurnIndex || (int)command.Player != _expectedTurn?.PlayerIndex) 
                throw new BattleLogicException("Wrong turn index or player index");
            
            await connection.SendMessageAsync(new CommandApproved(command));
            
            var serverCommand = new UnitPassCommandFromServer(command.TurnIndex, command.Player, command.ActorId);
            await BroadcastMessageToClientAndSaveAsync(serverCommand);
            
            _awaitPlayerTurnTaskCompletionSource?.SetResult(new ClientUserAction(command.Command));
        }

        private async Task OnClientUnitWaits(IBattleClientConnection connection, UnitWaitClientBattleMessage command)
        {
            var targetActor =
                BattleObject.Units.FirstOrDefault(x => x.Id.ToString() == command.ActorId);
            if (targetActor == null)
                throw new BattleLogicException("Not found target actor for client command");
            if (targetActor != _activeUnit)
                throw new BattleLogicException("Wrong target actor for client command");
            
            if (command.TurnIndex != _expectedTurn?.TurnIndex || (int)command.Player != _expectedTurn?.PlayerIndex) 
                throw new BattleLogicException("Wrong turn index or player index");
            
            if (targetActor.Waited)
                throw new BattleLogicException("Unit already performed wait command in the current round");
            
            if (_sortedBattleUnitObjects.Remove(targetActor))
                _sortedBattleUnitObjects.Add(targetActor);
            
            var mutableActor = targetActor;
            mutableActor.Waited = true;
            
            await connection.SendMessageAsync(new CommandApproved(command));
            
            var serverCommand = new UnitWaitCommandFromServer(command.TurnIndex, command.Player, command.ActorId);
            await BroadcastMessageToClientAndSaveAsync(serverCommand);
            
            await BattleUnitsService.UpdateUnits(new[] { mutableActor });
            
            _awaitPlayerTurnTaskCompletionSource?.SetResult(new ClientUserAction(command.Command));
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
            
            _awaitPlayerTurnTaskCompletionSource?.SetResult(new ClientUserAction(command.Command));
        }

        private async Task OnClientConnected(IBattleClientConnection connection, ClientConnectedBattleMessage message)
        {
            await connection.SendMessageAsync(new CommandApproved(message));
            
            for (int i = message.TurnIndex; i < BattleObject.TurnNumber; i++)
            {
                if (_passedServerBattleMessages.TryGetValue(i, out var messagesFromTurn))
                {
                    await Task.WhenAll(messagesFromTurn.Select(connection.SendMessageAsync));
                }
            }
        }
        
        private void SortByInitiative(List<MutableBattleUnitObject> units)
        {
            units.Sort((x, y) =>
            {
                // 1. Compare Waited status: false comes before true
                int waitedCompare = x.Waited.CompareTo(y.Waited);
                if (waitedCompare != 0)
                    return waitedCompare;

                // 2. Compare Speed: higher speed comes first
                int speedCompare = y.GlobalUnit.UnitType.Speed.CompareTo(x.GlobalUnit.UnitType.Speed);
                if (speedCompare != 0)
                    return speedCompare;

                // 3. Compare Row: lower slot number comes first
                var slotCompare = x.GlobalUnit.ContainerSlotIndex.CompareTo(y.GlobalUnit.ContainerSlotIndex);
                if (slotCompare != 0)
                    return slotCompare;
                
                // 3. Compare Side: lower player index comes first
                return x.PlayerIndex.CompareTo(y.PlayerIndex);
            });
        }
        
        private IBattleUnitObject GetActiveUnit(int lastTurnUnitIndex, out int activeUnitIndex)
        {
            int count = _sortedBattleUnitObjects.Count;

            for (int i = 1; i <= count; i++)
            {
                var nextIndex = (lastTurnUnitIndex + i) % count;
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