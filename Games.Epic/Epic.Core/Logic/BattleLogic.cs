using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Epic.Core.ClientMessages;
using Epic.Core.Logic.Erros;
using Epic.Core.Objects.Battle;
using Epic.Core.Objects.BattleClientConnection;
using Epic.Core.Objects.BattleUnit;
using Epic.Core.ServerMessages;
using JetBrains.Annotations;

namespace Epic.Core.Logic
{
    public class BattleLogic : IBattleLogic
    {
        public event Action<IServerBattleMessage> BroadcastMessage;
        private MutableBattleObject BattleObject { get; }
        private IBattleUnitsService BattleUnitsService { get; }
        private IUserUnitsService UserUnitsService { get; }
        private IBattlesService BattlesService { get; }
        private IRewardsService RewardsService { get; }

        private readonly List<MutableBattleUnitObject> _sortedBattleUnitObjects;

        private readonly ConcurrentDictionary<int, List<IServerBattleMessage>> _passedServerBattleMessages = new ConcurrentDictionary<int, List<IServerBattleMessage>>();

        [CanBeNull] private TurnInfo _expectedTurn;
        [CanBeNull] private TaskCompletionSource<Task> _awaitPlayerTurnTaskCompletionSource;
        
        public BattleLogic(
            [NotNull] MutableBattleObject battleObject, 
            [NotNull] IBattleUnitsService battleUnitsService,
            [NotNull] IUserUnitsService userUnitsService,
            [NotNull] IBattlesService battlesService,
            [NotNull] IRewardsService rewardsService)
        {
            BattleObject = battleObject ?? throw new ArgumentNullException(nameof(battleObject));
            BattleUnitsService = battleUnitsService ?? throw new ArgumentNullException(nameof(battleUnitsService));
            UserUnitsService = userUnitsService ?? throw new ArgumentNullException(nameof(userUnitsService));
            BattlesService = battlesService ?? throw new ArgumentNullException(nameof(battlesService));
            RewardsService = rewardsService ?? throw new ArgumentNullException(nameof(rewardsService));

            _sortedBattleUnitObjects = new List<MutableBattleUnitObject>(battleObject.Units);
            _sortedBattleUnitObjects.Sort((x, y) => x.UserUnit.UnitType.Speed.CompareTo(y.UserUnit.UnitType.Speed));
        }
        
        public void Dispose()
        {
            _sortedBattleUnitObjects.Clear();
            _awaitPlayerTurnTaskCompletionSource?.TrySetCanceled();
            _passedServerBattleMessages.Clear();
        }

        public async Task<BattleResult> Run(CancellationToken cancellationToken)
        {
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
                    var serverCommand = new NextTurnCommandFromServer
                    {
                        CommandId = Guid.NewGuid().ToString(),
                        TurnNumber = BattleObject.TurnIndex,
                        Player = ((InBattlePlayerNumber)activeUnit.PlayerIndex).ToString(),
                    };
                    BroadcastMessageToClientAndSave(serverCommand);
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
            }

            if (battleResult.Finished)
            {
                if (battleResult.Winner != null)
                {
                    var winnerUserId = BattleObject.GetPlayerUserId(battleResult.Winner.Value);
                    if (winnerUserId.HasValue)
                    {
                        var rewards =
                            await RewardsService.GetRewardsFromBattleDefinition(BattleObject.BattleDefinitionId);
                        var rewardsIds = rewards.Select(x => x.Id).ToArray();
                        await RewardsService.GiveRewardsToUserAsync(rewardsIds, winnerUserId.Value);
                    }
                }

                var battleFinishedCommand = new BattleFinishedCommandFromServer
                {
                    CommandId = Guid.NewGuid().ToString(),
                    TurnNumber = BattleObject.TurnIndex,
                    Winner = battleResult.Winner?.ToString() ?? string.Empty,
                };
                BroadcastMessageToClientAndSave(battleFinishedCommand);

                await BattlesService.FinishBattle(BattleObject, battleResult);
            }
            
            return battleResult;
        }

        private void BroadcastMessageToClientAndSave(IServerBattleMessage message)
        {
            var turnMessages = _passedServerBattleMessages.GetOrAdd(message.TurnNumber, _ => new List<IServerBattleMessage>());
            turnMessages.Add(message);
            BroadcastMessage?.Invoke(message);
        }

        private BattleResult GetBattleResult()
        {
            var noAliveUnits = true;
            InBattlePlayerNumber? winner = null;
            foreach (var battleUnitObject in _sortedBattleUnitObjects)
            {
                if (battleUnitObject.UserUnit.IsAlive)
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

            var serverMoveCommand = new UnitMoveCommandFromServer
            {
                CommandId = Guid.NewGuid().ToString(),
                TurnNumber = command.TurnIndex,
                Player = command.Player.ToString(),
                ActorId = command.ActorId,
                MoveToCell = command.MoveToCell
            };
            BroadcastMessageToClientAndSave(serverMoveCommand);

            var serverUnitAttackCommand = new UnitAttackCommandFromServer
            {
                CommandId = Guid.NewGuid().ToString(),
                TurnNumber = command.TurnIndex,
                Player = command.Player.ToString(),
                ActorId = command.ActorId,
                TargetId = command.TargetId,
            };
            BroadcastMessageToClientAndSave(serverUnitAttackCommand);
            
            var unitTakesDamageData = UnitTakesDamageData.FromUnitAndTarget(targetActor, targetTarget);
            targetTarget.UserUnit.Count = unitTakesDamageData.RemainingCount;
            targetTarget.UserUnit.IsAlive = targetTarget.UserUnit.Count > 0;

            await UserUnitsService.UpdateUnits(new [] { targetTarget.UserUnit });

            targetTarget.CurrentHealth = unitTakesDamageData.RemainingHealth;
            
            await BattleUnitsService.UpdateUnits(new[] { targetTarget });
                
            var serverUnitTakesDamage = new UnitTakesDamageCommandFromServer
            {
                CommandId = Guid.NewGuid().ToString(),
                TurnNumber = command.TurnIndex,
                Player = command.Player.ToString(),
                ActorId = command.TargetId,
                DamageTaken = unitTakesDamageData.DamageTaken,
                KilledCount = unitTakesDamageData.KilledCount,
                RemainingCount = unitTakesDamageData.RemainingCount,
                RemainingHealth = unitTakesDamageData.RemainingHealth,
            };
            BroadcastMessageToClientAndSave(serverUnitTakesDamage);
            
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
            
            var serverCommand = new UnitMoveCommandFromServer
            {
                CommandId = Guid.NewGuid().ToString(),
                TurnNumber = command.TurnIndex,
                Player = command.Player.ToString(),
                ActorId = command.ActorId,
                MoveToCell = command.MoveToCell
            };
            BroadcastMessageToClientAndSave(serverCommand);
            
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
            while (!activeUnit.UserUnit.IsAlive)
            {
                turnIndex++;
                turnIndex %= _sortedBattleUnitObjects.Count;
                activeUnit = _sortedBattleUnitObjects[turnIndex];
            }
            return activeUnit;
        }
    }
}