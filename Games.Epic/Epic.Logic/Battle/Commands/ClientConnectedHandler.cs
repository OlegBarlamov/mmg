using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epic.Core;
using Epic.Core.ClientMessages;
using Epic.Core.ServerMessages;
using Epic.Core.Services.GameManagement;
using JetBrains.Annotations;

namespace Epic.Logic.Battle.Commands
{
    internal class ClientConnectedHandler : BaseTypedCommandHandler<ClientConnectedBattleMessage>, IBattleMessageBroadcaster, IDisposable
    {
        private IBattleMessageBroadcaster MessageBroadcaster { get; }
        private readonly ConcurrentDictionary<int, List<IServerBattleMessage>> _passedServerBattleMessages =
            new ConcurrentDictionary<int, List<IServerBattleMessage>>();

        private bool _clientConnected;
        [CanBeNull] private TaskCompletionSource<bool> _awaitFirstClientConnectedCompletionSource;
        
        public void Dispose()
        {
            _passedServerBattleMessages.Clear();
            _awaitFirstClientConnectedCompletionSource?.TrySetCanceled();
        }
        
        public ClientConnectedHandler([NotNull] IBattleMessageBroadcaster messageBroadcaster)
        {
            MessageBroadcaster = messageBroadcaster ?? throw new ArgumentNullException(nameof(messageBroadcaster));
        }

        public Task WaitForFirstClientConnected()
        {
            if (_clientConnected) return Task.CompletedTask;
            
            _awaitFirstClientConnectedCompletionSource ??= new TaskCompletionSource<bool>();
            return _awaitFirstClientConnectedCompletionSource.Task;
        }
        
        public override Task Validate(CommandExecutionContext context, ClientConnectedBattleMessage command)
        {
            return Task.CompletedTask;
        }
        
        public Task BroadcastMessageToClientAndSaveAsync(IServerBattleMessage message)
        {
            var turnMessages = _passedServerBattleMessages.GetOrAdd(
                message.TurnNumber, _ => new List<IServerBattleMessage>());
            
            turnMessages.Add(message);
            
            return MessageBroadcaster.BroadcastMessageAsync(message);
        }
        

        public override async Task<ICmdExecutionResult> Execute(CommandExecutionContext context, ClientConnectedBattleMessage command)
        {
            for (int i = command.TurnIndex; i < context.BattleObject.TurnNumber; i++)
            {
                if (_passedServerBattleMessages.TryGetValue(i, out var messagesFromTurn))
                {
                    await Task.WhenAll(messagesFromTurn.Select(context.Connection.SendMessageAsync));
                }
            }

            if (context.UnitsCarousel.ActiveUnit != null)
            {
                var activeUnit = context.UnitsCarousel.ActiveUnit;
                var unitCanAct = activeUnit.CanAct;
                
                await context.Connection.SendMessageAsync(new NextTurnCommandFromServer(
                    context.BattleObject.TurnNumber,
                    context.BattleObject.TurnPlayerIndex.ToInBattlePlayerNumber(),
                    activeUnit.Id,
                    context.BattleObject.RoundNumber,
                    unitCanAct));
            }

            _clientConnected = true;
            var completionSource = _awaitFirstClientConnectedCompletionSource;
            _awaitFirstClientConnectedCompletionSource = null;
            completionSource?.TrySetResult(true);
            return new CmdExecutionResult(false);
        }

        Task IBattleMessageBroadcaster.BroadcastMessageAsync(IServerBattleMessage message)
        {
            return BroadcastMessageToClientAndSaveAsync(message);
        }
    }
}