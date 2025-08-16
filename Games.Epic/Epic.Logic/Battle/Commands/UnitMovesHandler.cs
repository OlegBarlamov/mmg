using System.Threading.Tasks;
using Epic.Core.ClientMessages;
using Epic.Core.ServerMessages;

namespace Epic.Logic.Battle.Commands
{
    internal class UnitMovesHandler : BaseTypedCommandHandler<UnitMoveClientBattleMessage>
    {
        public override void Validate(CommandExecutionContext context, UnitMoveClientBattleMessage command)
        {
            ValidateTargetActor(context, command.ActorId);
            ValidateExpectedTurn(context, command.TurnIndex, command.Player);
            
            //TODO Check if it is reachable
        }

        public override async Task<ICmdExecutionResult> Execute(CommandExecutionContext context, UnitMoveClientBattleMessage command)
        {
            TargetActor.Column = command.MoveToCell.C;
            TargetActor.Row = command.MoveToCell.R;

            await context.BattleUnitsService.UpdateUnits(new[] { TargetActor });

            var serverCommand = new UnitMoveCommandFromServer(command.TurnIndex, command.Player, command.ActorId,
                command.MoveToCell);
            await context.MessageBroadcaster.BroadcastMessageAsync(serverCommand);

            return new CmdExecutionResult(true);
        }
    }
}