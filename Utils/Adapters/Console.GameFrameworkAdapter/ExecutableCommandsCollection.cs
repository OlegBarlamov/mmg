using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Console.Core.Commands;
using FrameworkSDK.DependencyInjection;
using JetBrains.Annotations;

namespace Console.FrameworkAdapter
{
    public class ExecutableCommandsCollection : IExecutableCommandsCollection
    {
        [NotNull] private IServiceLocator ServiceLocator { get; }
        private bool _commandsLoaded;
        private readonly ConcurrentDictionary<string, IExecutableConsoleCommand> _registeredCommands;

        public ExecutableCommandsCollection([NotNull] IServiceLocator serviceLocator)
        {
            ServiceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));

            _registeredCommands = new ConcurrentDictionary<string, IExecutableConsoleCommand>();
        }

        public IReadOnlyDictionary<string, IExecutableConsoleCommand> GetAvailableCommands()
        {
            if (!_commandsLoaded)
                PreloadCommands();
            
            return _registeredCommands;
        }

        public void PreloadCommands()
        {
            _commandsLoaded = true;

            var commands = ServiceLocator.ResolveMultiple<IExecutableConsoleCommand>();
            foreach (var consoleCommand in commands)
            {
                _registeredCommands.AddOrUpdate(consoleCommand.Text, consoleCommand, (text, command) => command);
            }
        }

        public void AddCommand([NotNull] IExecutableConsoleCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            _registeredCommands.AddOrUpdate(command.Text, command, (s, consoleCommand) => command);
        }

        public void RemoveCommand([NotNull] IExecutableConsoleCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            _registeredCommands.TryRemove(command.Text, out _);
        }
    }
}