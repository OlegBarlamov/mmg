using System.Threading.Tasks;
using Epic.Core.ClientMessages;
using Epic.Core.Logic.Erros;
using Epic.Core.ServerMessages;

namespace Epic.Logic.Battle.Commands
{
    internal class UnitWaitsHandler : BaseTypedCommandHandler<UnitWaitClientBattleMessage>
    {
        public override Task Validate(CommandExecutionContext context, UnitWaitClientBattleMessage command)
        {
            ValidateTargetActor(context, command.ActorId);
            ValidateExpectedTurn(context, command.TurnIndex, command.Player);
            
            if (TargetActor.Waited)
                throw new BattleLogicException("Unit already performed wait command in the current round");
            
            return Task.CompletedTask;
        }

        public override async Task<ICmdExecutionResult> Execute(CommandExecutionContext context, UnitWaitClientBattleMessage command)
        {
            TargetActor.Waited = true;
            
            await context.BattleUnitsService.UpdateUnits(new[] { TargetActor });
            
            var serverCommand = new UnitWaitCommandFromServer(command.TurnIndex, command.Player, command.ActorId);
            await context.MessageBroadcaster.BroadcastMessageAsync(serverCommand);
            
            context.UnitsCarousel.ActiveUnitWaits();
            
            return new CmdExecutionResult(true);
        }
    }
}