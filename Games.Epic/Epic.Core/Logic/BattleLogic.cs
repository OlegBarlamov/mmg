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
        public IBattlesService BattlesService { get; }

        private readonly List<MutableBattleUnitObject> _sortedBattleUnitObjects;

        private readonly ConcurrentDictionary<int, List<IServerBattleMessage>> _passedServerBattleMessages = new ConcurrentDictionary<int, List<IServerBattleMessage>>();

        [CanBeNull] private TurnInfo _expectedTurn;
        [CanBeNull] private TaskCompletionSource<Task> _awaitPlayerTurnTaskCompletionSource;
        
        public BattleLogic(
            [NotNull] MutableBattleObject battleObject, 
            [NotNull] IBattleUnitsService battleUnitsService,
            [NotNull] IBattlesService battlesService)
        {
            BattleObject = battleObject;
            BattleUnitsService = battleUnitsService ?? throw new ArgumentNullException(nameof(battleUnitsService));
            BattlesService = battlesService ?? throw new ArgumentNullException(nameof(battlesService));

            _sortedBattleUnitObjects = new List<MutableBattleUnitObject>(battleObject.Units);
            _sortedBattleUnitObjects.Sort((x, y) => x.UserUnit.UnitType.Speed.CompareTo(y.UserUnit.UnitType.Speed));
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            try
            {
                var activeUnit = GetActiveUnit(BattleObject.TurnIndex);
                while (true)
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

                    var winner = GetWinner();
                    if (winner != null)
                    {
                        // Finish the battle
                    }
                    
                    BattleObject.TurnIndex++;
                    await BattlesService.UpdateBattle(BattleObject);

                    activeUnit = GetActiveUnit(BattleObject.TurnIndex);
                    var serverCommand = new NextTurnCommandFromServer
                    {
                        CommandId = Guid.NewGuid().ToString(),
                        TurnNumber = BattleObject.TurnIndex,
                        Player = ((PlayerNumber)activeUnit.PlayerIndex).ToString(),
                    };
                    BroadcastMessageToClientAndSave(serverCommand);
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
        }

        private void BroadcastMessageToClientAndSave(IServerBattleMessage message)
        {
            var turnMessages = _passedServerBattleMessages.GetOrAdd(message.TurnNumber, _ => new List<IServerBattleMessage>());
            turnMessages.Add(message);
            BroadcastMessage?.Invoke(message);
        }

        private PlayerNumber? GetWinner()
        {
            return null;
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
                default:
                    throw new ClientCommandRejected("Unknown client command");
            }
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