using System;
using Console.Core;
using Console.Core.CommandExecution;
using Console.Core.Implementations.Terminal;
using FrameworkSDK;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace Console.FrameworkAdapter.Constructing
{
    [UsedImplicitly]
    public class TerminalConsoleSubsystem : IAppSubSystem
    {
        private IConsoleCommandExecutor ConsoleCommandExecutor { get; }
        private IFrameworkLogger Logger { get; }
        private IExecutableCommandsCollection ExecutableCommandsCollection { get; }
        private IConsoleController ConsoleController { get; }
        private readonly TerminalConsoleCommandListener _terminalConsoleCommandListener;
        
        public TerminalConsoleSubsystem([NotNull] IConsoleCommandExecutor consoleCommandExecutor,
            [NotNull] IFrameworkLogger logger, [NotNull] IExecutableCommandsCollection executableCommandsCollection,
            [NotNull] IConsoleController consoleController)
        {
            ConsoleCommandExecutor = consoleCommandExecutor ?? throw new ArgumentNullException(nameof(consoleCommandExecutor));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ExecutableCommandsCollection = executableCommandsCollection ?? throw new ArgumentNullException(nameof(executableCommandsCollection));
            ConsoleController = consoleController ?? throw new ArgumentNullException(nameof(consoleController));
            _terminalConsoleCommandListener = new TerminalConsoleCommandListener();
        }
        
        public void Dispose()
        {
            _terminalConsoleCommandListener.Command -= TerminalConsoleCommandListenerOnCommand;
            _terminalConsoleCommandListener.Dispose();
        }

        public void Configure()
        {
            _terminalConsoleCommandListener.Command += TerminalConsoleCommandListenerOnCommand;
            ExecutableCommandsCollection.PreloadCommands();
            Logger.Log($"Commands preloaded: {ExecutableCommandsCollection.GetAvailableCommands().Count}", "Commands", FrameworkLogLevel.Info);
        }

        private async void TerminalConsoleCommandListenerOnCommand(string command)
        {
            try
            {
                await ConsoleCommandExecutor.ExecuteAsync(command);
            }
            catch (Exception e)
            {
                Logger.Log($"Error during command '{command}' execution: {e}", "Commands", FrameworkLogLevel.Error);
            }
        }

        public void Run()
        {
            _terminalConsoleCommandListener.StartListen();
        }
    }
}