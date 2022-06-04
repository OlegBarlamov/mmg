using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Console.Core;
using Console.Core.Models;
using Console.FrameworkAdapter.Commands;
using FrameworkSDK.DependencyInjection;
using FrameworkSDK.Logging;
using JetBrains.Annotations;

namespace Console.FrameworkAdapter
{
    [UsedImplicitly]
    public class ExecutableConsoleCommandsExecutor : IConsoleCommandExecutor
    {
        private IFrameworkLogger Logger { get; }
        private IExecutableCommandsCollection ExecutableCommandsCollection { get; }

        public ExecutableConsoleCommandsExecutor([NotNull] IFrameworkLogger logger,
            [NotNull] IExecutableCommandsCollection executableCommandsCollection)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ExecutableCommandsCollection = executableCommandsCollection ?? throw new ArgumentNullException(nameof(executableCommandsCollection));
        }

        public IEnumerable<IConsoleCommand> GetAvailableCommands()
        {
            return ExecutableCommandsCollection.GetAvailableCommands().Values;
        }

        public async Task ExecuteAsync(string command)
        {
            var commandName = command.Split(' ')[0];
            var commands = ExecutableCommandsCollection.GetAvailableCommands();
            if (!commands.ContainsKey(commandName))
            {
                Logger.Log($"Unknown user command: {commandName}", "Console", FrameworkLogLevel.Info);
                return;
            }

            try
            {
                await commands[commandName].ExecuteAsync(command);
            }
            catch (ConsoleCommandExecutionException e)
            {
                Logger.Log(e.Message, "Console", FrameworkLogLevel.Info);
            }
        }
    }
}