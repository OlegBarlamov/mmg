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
        
        public void Dispose()
        {
            _passedServerBattleMessages.Clear();
        }
        
        public ClientConnectedHandler([NotNull] IBattleMessageBroadcaster messageBroadcaster)
        {
            MessageBroadcaster = messageBroadcaster ?? throw new ArgumentNullException(nameof(messageBroadcaster));
        }
        
        public override void Validate(CommandExecutionContext context, ClientConnectedBattleMessage command)
        {
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
                await context.Connection.SendMessageAsync(new NextTurnCommandFromServer(
                    context.BattleObject.TurnNumber,
                    context.BattleObject.TurnPlayerIndex.ToInBattlePlayerNumber(),
                    context.UnitsCarousel.ActiveUnit.Id,
                    context.BattleObject.RoundNumber));
            }

            return new CmdExecutionResult(false);
        }

        Task IBattleMessageBroadcaster.BroadcastMessageAsync(IServerBattleMessage message)
        {
            return BroadcastMessageToClientAndSaveAsync(message);
        }
    }
}