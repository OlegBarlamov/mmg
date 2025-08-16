using System.Threading.Tasks;
using Epic.Core.ClientMessages;

namespace Epic.Logic.Battle.Commands
{
    internal abstract class BaseTypedCommandHandler<TCommand> : BaseCommandHandler, ITypedCommandHandler<TCommand>
        where TCommand : IClientBattleMessage
    {
        void ICommandsHandler.Validate(CommandExecutionContext context, IClientBattleMessage command)
        {
            Validate(context, (TCommand)command);
        }

        Task<ICmdExecutionResult> ICommandsHandler.Execute(CommandExecutionContext context, IClientBattleMessage command)
        {
            return Execute(context, (TCommand)command);
        }

        public abstract void Validate(CommandExecutionContext context, TCommand command);

        public abstract Task<ICmdExecutionResult> Execute(CommandExecutionContext context, TCommand command);
    }
}