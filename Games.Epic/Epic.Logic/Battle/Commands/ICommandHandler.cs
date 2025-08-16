using System.Threading.Tasks;
using Epic.Core.ClientMessages;

namespace Epic.Logic.Battle.Commands
{
    internal interface ICmdExecutionResult
    {
        bool TurnFinished { get; set; }
    }

    internal interface ICommandsHandler
    {
        void Validate(CommandExecutionContext context, IClientBattleMessage command);
        
        Task<ICmdExecutionResult> Execute(CommandExecutionContext context, IClientBattleMessage command);
    } 
    
    internal interface ITypedCommandHandler<in TCommand> : ICommandsHandler where TCommand : IClientBattleMessage 
    {
        void Validate(CommandExecutionContext context, TCommand command);
        
        Task<ICmdExecutionResult> Execute(CommandExecutionContext context, TCommand command);
    }
}