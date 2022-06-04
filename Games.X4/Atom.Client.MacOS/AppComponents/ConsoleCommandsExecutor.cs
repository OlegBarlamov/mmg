using System;
using Console.FrameworkAdapter;
using FrameworkSDK;
using FrameworkSDK.MonoGame.Services;
using JetBrains.Annotations;

namespace Atom.Client.MacOS.AppComponents
{
    public class ConsoleCommandsExecutor : IAppComponent
    {
        private CommandExecutorMediator CommandExecutorMediator { get; }
        private IAppTerminator AppTerminator { get; }

        public ConsoleCommandsExecutor([NotNull] CommandExecutorMediator commandExecutorMediator,
            [NotNull] IAppTerminator appTerminator)
        {
            CommandExecutorMediator = commandExecutorMediator ?? throw new ArgumentNullException(nameof(commandExecutorMediator));
            AppTerminator = appTerminator ?? throw new ArgumentNullException(nameof(appTerminator));
        }
        
        public void Dispose()
        {
            CommandExecutorMediator.Command -= CommandExecutorMediatorOnCommand;
        }

        public void Configure()
        {
            CommandExecutorMediator.Command += CommandExecutorMediatorOnCommand;
        }

        private void CommandExecutorMediatorOnCommand(string command)
        {
            if (command == "exit")
            {
                AppTerminator.Terminate();
            }
        }
    }
}