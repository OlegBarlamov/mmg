using System.Threading.Tasks;
using Epic.Core.ClientMessages;

namespace Epic.Logic.Battle.Commands
{
    internal abstract class BaseTypedCommandHandler<TCommand> : BaseCommandHandler, ITypedCommandHandler<TCommand>
        where TCommand : IClientBattleMessage
    {
        Task ICommandsHandler.Validate(CommandExecutionContext context, IClientBattleMessage command)
        {
            return Validate(context, (TCommand)command);
        }

        Task<ICmdExecutionResult> ICommandsHandler.Execute(CommandExecutionContext context, IClientBattleMessage command)
        {
            return Execute(context, (TCommand)command);
        }

        public abstract Task Validate(CommandExecutionContext context, TCommand command);

        public abstract Task<ICmdExecutionResult> Execute(CommandExecutionContext context, TCommand command);
    }
}