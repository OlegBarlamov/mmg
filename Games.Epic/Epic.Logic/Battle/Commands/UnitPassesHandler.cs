using System.Threading.Tasks;
using Epic.Core.ClientMessages;
using Epic.Core.ServerMessages;

namespace Epic.Logic.Battle.Commands
{
    internal class UnitPassesHandler : BaseTypedCommandHandler<UnitPassClientBattleMessage>
    {
        public override void Validate(CommandExecutionContext context, UnitPassClientBattleMessage command)
        {
            ValidateTargetActor(context, command.ActorId);
            ValidateExpectedTurn(context, command.TurnIndex, command.Player);
        }

        public override async Task<ICmdExecutionResult> Execute(CommandExecutionContext context, UnitPassClientBattleMessage command)
        {
            var serverCommand = new UnitPassCommandFromServer(command.TurnIndex, command.Player, command.ActorId);
            await context.MessageBroadcaster.BroadcastMessageAsync(serverCommand);
            
            return new CmdExecutionResult(true);
        }
    }
}