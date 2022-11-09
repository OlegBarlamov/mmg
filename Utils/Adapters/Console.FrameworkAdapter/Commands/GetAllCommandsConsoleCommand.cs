using System;
using System.Threading.Tasks;
using Console.Core;
using Console.Core.CommandExecution;
using Console.Core.Commands;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace Console.FrameworkAdapter.Commands
{
    [RegisterConsoleCommand]
    public class GetAllCommandsConsoleCommand : FixedTypedExecutableConsoleCommand
    {
        [NotNull] public IExecutableCommandsCollection ExecutableCommandsCollection { get; }
        [NotNull] public IConsoleController ConsoleController { get; }
        public override string Text { get; } = "?";
        public override string Description { get; } = "Output all registered commands";

        public GetAllCommandsConsoleCommand([NotNull] IExecutableCommandsCollection executableCommandsCollection, [NotNull] IConsoleController consoleController)
        {
            ExecutableCommandsCollection = executableCommandsCollection ?? throw new ArgumentNullException(nameof(executableCommandsCollection));
            ConsoleController = consoleController ?? throw new ArgumentNullException(nameof(consoleController));
        }
        
        protected override Task ExecuteAsync()
        {
            var allCommands = ExecutableCommandsCollection.GetAvailableCommands();

            foreach (var command in allCommands)
            {
                ConsoleController.AddMessage($"{command.Value.Metadata?.Title ?? command.Key} --- {command.Value.Metadata?.Description ?? "No description"}");
            }
            
            return Task.CompletedTask;
        }
    }
}